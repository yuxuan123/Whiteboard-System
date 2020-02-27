/* eslint-disable no-console */
// https://vuex.vuejs.org/en/actions.html
import axios from 'axios'

// The login action passes vuex commit helper that we will use to trigger mutations.
export default {
  LOGIN({ commit }, userData) {
    return new Promise((resolve, reject) => {
      resolve(true);
    })
  },
  LOGOUT({ commit }) {
    return new Promise((resolve, reject) => {
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
