/// <binding />
"use strict";
var path = require('path');
var webpack = require('webpack');
var CleanWebpackPlugin = require('clean-webpack-plugin');
var AssetsPlugin = require('assets-webpack-plugin');
var BeepPlugin = require('webpack-beep-plugin');
var node_modules = path.join(__dirname, "node_modules");
var ForkCheckerPlugin = require('awesome-typescript-loader').ForkCheckerPlugin;


module.exports = {
    entry: {
        main: [ "./Scripts/Main.tsx" ],
        "react": ["react", "react-bootstrap", "react-router", "react-widgets", "react-router-bootstrap"],
        "moment": ["moment"],
        "numbro": ["numbro"],
    },
    output: {
        path: path.join(__dirname, "dist"),
        filename: "bundle.[name].[chunkhash].js",
        chunkFilename: "bundle.[name].[chunkhash].js"
    },
    resolve: {
        root: node_modules,
        extensions: ['', '.Webpack.js', '.web.js', '.ts', '.js', '.tsx']     
    },
    resolveLoader: { root: path.join(__dirname, "node_modules") },
    module: {
        //noParse : [
        //    //node_modules + "/react/dist/react.min.js",
        //    node_modules + "/react-dom/dist/react-dom.min.js",
        //    node_modules + "/react-bootstrap/dist/react-bootstrap.min.js",
        //    node_modules + "/react-router/umd/ReactRouter.min.js",
        //    node_modules + "/react-widgets/dist/react-widgets.js"
        //],
        loaders: [
            //{ test: /\.tsx?$/, loader: 'awesome-typescript-loader' },
            { test: /\.tsx?$/, loader: 'ts-loader' },
            //{ test: /\.tsx?$/, loader: 'happypack/loader' },
            //{ test: /\.jsx?$/, loader: "babel-loader" }
            { test: /\.css$/, loader: "style-loader!css-loader" },
            { test: /\.less$/, loader: "style-loader!css-loader!less-loader" },
            { test: /\.gif$/, loader: "url-loader?mimetype=image/png" },
            { test: /\.woff(2)?(\?v=[0-9].[0-9].[0-9])?$/, loader: "url-loader?mimetype=application/font-woff" },
            { test: /\.(ttf|eot|svg)(\?v=[0-9].[0-9].[0-9])?$/, loader: "file-loader?name=[name].[ext]" },
        ]
    },
    plugins: [
        //new webpack.optimize.LimitChunkCountPlugin({maxChunks: 15}),
        new webpack.optimize.CommonsChunkPlugin({
            names: ['react', 'numbro', 'moment'],
        }),
        //new webpack.optimize.UglifyJsPlugin(),
        new webpack.ContextReplacementPlugin(/moment[\/\\]locale$/, /(en|es)/),
        new CleanWebpackPlugin(['dist'], {
            verbose: true,
            dry: false
        }),
        new AssetsPlugin({
            path: path.join(__dirname, 'dist')
        }),
        new BeepPlugin(),
        //new ForkCheckerPlugin(),
    ],
    devtool: false,
    ts: {
        transpileOnly: true,
        compilerOptions: {
            "noEmit": false
        }
    }
}