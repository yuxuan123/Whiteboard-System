/* eslint-disable no-console */
// https://vuex.vuejs.org/en/actions.html
import axios from 'axios'
import { API_URL } from "@/utils/config";

// The login action passes vuex commit helper that we will use to trigger mutations.
export default {
  LOGIN({ commit }, userData) {
    return new Promise((resolve, reject) => {
      axios.post(API_URL + '/authenticate', { username: userData.username, password: userData.password })
        .then(response => {
          const username = response.data.userName
          const userid = response.data.userId;
          const role = response.data.role;
          $cookies.set("username", username, Infinity)
          $cookies.set("userid", userid, Infinity)
          $cookies.set("role", role, Infinity)
          $cookies.set("authenticated", true, Infinity);
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },
  LOGOUT({ commit }) {
    return new Promise((resolve, reject) => {
      $cookies.remove('username')
      $cookies.remove('userid')
      $cookies.remove('role')
      $cookies.remove('authenticated')
      resolve(true);
    })
  },
  GETUSER({ commit }, userid) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
  CHANGEPASSWORD({ commit }, payload) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
  SETNEWPASSWORD({ commit }, payload) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
  ACTIVATE({ commit }, email) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
  RESET({ commit }, email) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
}
