// https://vuex.vuejs.org/en/getters.html
export default {
  // authorized lets you know if the token is true or not
  authorized: state => !!state.accessToken,
  authstatus: state => state.authStatus,
  user: (state) => {
    return state.username;
  }
};
