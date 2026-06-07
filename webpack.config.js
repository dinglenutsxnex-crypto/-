const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const isDev = process.env.NODE_ENV !== "production";

module.exports = {
  entry: "./src/scripts/core/main.ts",

  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "js/[name].[contenthash].js",
    clean: true,
  },

  resolve: {
    extensions: [".ts", ".js"],
    alias: {
      "@scripts": path.resolve(__dirname, "src/scripts"),
      "@ui":      path.resolve(__dirname, "src/ui"),
      "@assets":  path.resolve(__dirname, "src/assets"),
      "@plugins": path.resolve(__dirname, "src/plugins"),
    },
    fallback: {
      "fs": false,
      "path": false,
    },
  },

  module: {
    rules: [
      {
        test: /\.ts$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
      {
        test: /\.css$/,
        use: isDev
          ? ["style-loader", "css-loader"]
          : [
              { loader: MiniCssExtractPlugin.loader, options: { publicPath: "../" } },
              { loader: "css-loader", options: { url: false } },
            ],
      },
    ],
  },

  plugins: [
    new HtmlWebpackPlugin({
      template: "./src/ui/index.html",
      filename: "index.html",
    }),

    new CopyWebpackPlugin({
      patterns: [
        { from: "src/assets", to: "assets" },
        { from: "src/plugins", to: "plugins" },
        { from: "src/ui/screens", to: "ui/screens" },
      ],
    }),

    ...(isDev ? [] : [
      new MiniCssExtractPlugin({ filename: "css/[name].[contenthash].css" }),
    ]),
  ],

  devServer: {
    static: "./dist",
    port: 3000,
    hot: true,
    open: true,
  },

  optimization: {
    splitChunks: {
      chunks: "all",
      cacheGroups: {
        babylon: {
          test: /[\\/]node_modules[\\/]@babylonjs[\\/]/,
          name: "babylon",
          priority: 20,
        },
        vendors: {
          test: /[\\/]node_modules[\\/]/,
          name: "vendors",
          priority: 10,
        },
      },
    },
  },
};
