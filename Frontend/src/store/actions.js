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
  
  GETALLUSER({ commit }, pagination) {
    return new Promise((resolve, reject) => {
      const { page, rowsPerPage, sortBy, descending, search } = pagination;
      //Apply asc/desc to sorting Name
      if (descending != true && descending != undefined) {
        sortBy = sortBy + " desc";
      }
      // Removed Params for now. Waiting for Cors
      // { params: { PageNumber: page, PageSize: rowsPerPage, OrderBy: sortBy, keyword: search } }
      axios.get(API_URL + '/getAllUsers')
        .then(response => {
          resolve(response)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  CREATEUSER({ commit }, user){
    //Hardcode Password
    user.password = "user1234!";
    return new Promise((resolve, reject) => {
      axios.post(API_URL + '/createUser', user)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  UPDATEUSER({ commit }, user){
    return new Promise((resolve, reject) => {
      axios.put(API_URL + '/' + user.userId, user)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  DELETEUSER({ commit }, user){
    return new Promise((resolve, reject) => {
      axios.delete(API_URL + '/' + user.userId, user)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
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
