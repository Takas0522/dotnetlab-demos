const path = require('path');

module.exports = {
    mode: 'development',
    entry: './scripts/index.ts',
    output: {
        path: path.join(__dirname, "dist"),
        filename: "index.bundle.js"
    },
    module: {
        rules: [{
            test: /\.ts$/,
            use: 'ts-loader'
        }]
    },
    resolve: {
        modules: [
            "node_modules",
        ],
        extensions: [
            '.ts',
            '.js'
        ]
    }
};