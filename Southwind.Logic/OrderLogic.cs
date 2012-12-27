﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Engine;
using Signum.Engine.DynamicQuery;
using Southwind.Entities;
using System.Reflection;
using Signum.Utilities;
using Signum.Engine.Operations;
using Signum.Entities.Authorization;

namespace Southwind.Logic
{
    public static class OrderLogic
    {
        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<OrderDN>();

                dqm[typeof(OrderDN)] = (from o in Database.Query<OrderDN>()
                                        select new
                                        {
                                            Entity = o.ToLite(),
                                            o.Id,
                                            o.State,
                                            Customer = o.Customer.ToLite(),
                                            o.Employee,
                                            o.OrderDate,
                                            o.RequiredDate,
                                            o.ShipAddress,
                                            o.ShipVia,
                                        }).ToDynamic();

                dqm[OrderQueries.OrderLines] = (from o in Database.Query<OrderDN>()
                                                from od in o.Details
                                                select new
                                                {
                                                    Entity = o.ToLite(),
                                                    o.Id,
                                                    od.Product,
                                                    od.Quantity,
                                                    od.UnitPrice,
                                                    od.Discount,
                                                    od.SubTotalPrice,
                                                }).ToDynamic();

             

                GraphOrder.Register();
            }
        }

        public class GraphOrder : Graph<OrderDN, OrderState>
        {
            public static void Register()
            {
                GetState = o => o.State;

                new Construct(OrderOperation.Create)
                {
                    ToState = OrderState.New,
                    Construct = (_) => new OrderDN
                    {
                        State = OrderState.New,
                        Employee = EmployeeDN.Current.ToLite()
                    }
                }.Register();

                new ConstructFrom<CustomerDN>(OrderOperation.CreateOrderFromCustomer)
                {
                    ToState = OrderState.New,
                    Construct = (c, _) => new OrderDN
                    {
                        State = OrderState.New,
                        Customer = c,
                        Employee = EmployeeDN.Current.ToLite(),
                    }
                }.Register();

                new ConstructFromMany<ProductDN>(OrderOperation.CreateOrderFromProducts)
                {
                    ToState = OrderState.New,
                    Construct = (prods, _) =>
                    {
                        var dic = Database.Query<ProductDN>()
                            .Where(p => prods.Contains(p.ToLite()))
                            .Select(p => new KeyValuePair<Lite<ProductDN>, decimal>(p.ToLite(), p.UnitPrice)).ToDictionary();

                        return new OrderDN
                        {
                            State = OrderState.New,
                            Employee = EmployeeDN.Current.ToLite(),
                            Details = prods.Select(p => new OrderDetailsDN
                            {
                                Product = p,
                                UnitPrice = dic[p],
                                Quantity = 1,
                            }).ToMList()
                        };
                    }
                }.Register();

                new Execute(OrderOperation.SaveNew)
                {  
                    FromStates = new[] { OrderState.New },
                    ToState = OrderState.Ordered,
                    AllowsNew = true,
                    Lite = false,
                    Execute = (e, args) =>
                    {
                        e.OrderDate = DateTime.Now;
                        e.State = OrderState.Ordered; 
                    }
                }.Register();

                new Execute(OrderOperation.Save)
                {
                    FromStates = new[] { OrderState.Ordered },
                    ToState = OrderState.Ordered,
                    Lite = false,
                    Execute = (e, _) =>
                    {
                    }
                }.Register();

                new Execute(OrderOperation.Ship)
                {
                    CanExecute = o => o.Details.IsEmpty() ? "No order lines" : null,
                    FromStates = new[] { OrderState.Ordered },
                    ToState = OrderState.Shipped,
                    Execute = (e, args) =>
                    {
                        e.ShippedDate = DateTime.Now;
                        e.State = OrderState.Shipped;
                    }
                }.Register();

                new Execute(OrderOperation.Cancel)
                {
                    FromStates = new[] { OrderState.Ordered, OrderState.Shipped },
                    ToState = OrderState.Canceled,
                    Execute = (e, args) =>
                    {
                        e.CancelationDate = DateTime.Now;
                        e.State = OrderState.Canceled;
                    }
                }.Register();
            }
        }

        public static OrderDN Create(OrderDN order)
        {
            if (!order.IsNew)
                throw new ArgumentException("order should be new");

            using (Transaction tr = new Transaction())
            {
                foreach (var od in order.Details)
                {
                    int updated = od.Product.InDB().Where(p => p.UnitsInStock >= od.Quantity).UnsafeUpdate(p => new ProductDN
                    {
                        UnitsInStock = (short)(p.UnitsInStock - od.Quantity)
                    });


                    if (updated != 1)
                        throw new ApplicationException("There are not enought {0} in stock".Formato(od.Product));
                }

                order.Save();

                return tr.Commit(order);
            }
        }
    }
}