module.exports = {
  devServer: {
    host: "localhost",
    hot: true,
    port: 8080,
    open: "Chrome",
    disableHostCheck: true,
    // For handling requests starting with /api after BASE_URL
    proxy: {
      "^/api": {
        disableHostCheck: true,
        target: "http://localhost:5000",
        secure: false,
        changeOrigin: true
      }
    }
  }
};
