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

  /* User.vue */
  GETALLUSER({ commit }, pagination) {
    return new Promise((resolve, reject) => {
      const { page, rowsPerPage, sortBy, descending, search } = pagination;
      //Apply asc/desc to sorting Name
      var sortingName = sortBy;
      if (descending != true && descending != undefined) {
        sortingName = sortBy + " desc";
      }
      axios.get(API_URL + '/getAllUsers', { params: { PageNumber: page, PageSize: rowsPerPage, OrderBy: sortingName, keyword: search } })
        .then(response => {
          resolve(response)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  CREATEUSER({ commit }, user) {
    //Hardcode Password
    //user.password = "user1234!";
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

  UPDATEUSER({ commit }, user) {
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

  DELETEUSER({ commit }, user) {
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

  /* Post.vue */
  GETALLPOST({ commit }, pagination) {
    return new Promise((resolve, reject) => {
      const { page, rowsPerPage, sortBy, descending, search } = pagination;
      //Apply asc/desc to sorting Name
      var sortingName = sortBy;
      if (descending != true && descending != undefined) {
        sortingName = sortBy + " desc";
      }
      // Removed Params for now. Waiting for Cors
      // { params: { PageNumber: page, PageSize: rowsPerPage, OrderBy: sortBy, keyword: search } }
      axios.get(API_URL + '/getAllPosts', { params: { PageNumber: page, PageSize: rowsPerPage, OrderBy: sortBy, keyword: search } })
        .then(response => {
          resolve(response)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  CREATEDISCUSSION({ commit }, discussion) {
    return new Promise((resolve, reject) => {
      axios.post(API_URL + '/createPost', discussion)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  UPDATEDISCUSSION({commit}, discussion){
    return new Promise((resolve, reject) => {
      axios.post(API_URL + '/updatePost', discussion)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  DELETEDISCUSSION({ commit }, discussion) {
    return new Promise((resolve, reject) => {
      axios.delete(API_URL + '/deletePost/' + discussion.postId)
        .then(response => {
          resolve(true)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  /* Discussion.vue */
  GETALLCOURSES({ commit }) {
    return new Promise((resolve, reject) => {
      axios.get(API_URL + '/getAllCourses')
        .then(response => {
          resolve(response)
        })
        .catch(err => {
          reject(err)
        })
    })
  },

  GETALLCOURSEFOLDERS({ commit }) {
    return new Promise((resolve, reject) => {
      axios.get(API_URL + '/getAllCourseFolders')
        .then(response => {
          resolve(response)
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
