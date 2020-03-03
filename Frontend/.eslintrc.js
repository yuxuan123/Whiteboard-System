// ESLint Settings
module.exports = {
    root: true,
    env: {
        node: true
    },
    extends: [
        'prettier',
        'plugin:vue/recommended'
    ],
    rules: {
        'no-console': process.env.NODE_ENV === 'production' ? 'error' : 'off',
        'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off'
    },
    parserOptions: {
        parser: 'babel-eslint'
    }
}