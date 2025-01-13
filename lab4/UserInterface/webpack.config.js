const path = require("path");

module.exports = {
    entry: "app.fs.js",
    output: {
        path: path.resolve(__dirname, "dist"),
        filename: "bundle.js",
    },
    module: {
        rules: [
            {
                test: /\.fs.js$/,
                use: "fable-loader",
            },
        ],
    },
    devServer: {
        static: {
            directory: path.join(__dirname, "dist"),
        },
        compress: true,
        port: 8080,
    },
    mode: "development",
};
