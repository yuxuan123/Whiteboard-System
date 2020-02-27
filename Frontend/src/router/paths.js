/**
 * Define all of your application routes here
 * for more information on routes, see the
 * official documentation https://router.vuejs.org/en/
 */
import store from '../store'
// import axios from 'axios'
// import Cookies from 'js-cookie'

export default [
  {
    path: '*',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/error/NotFound.vue`)
  },
  {
    path: '/reset/:userId/:token',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/auth/Reset.vue`)
  },
  {
    path: '/activate/:userId/:token',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/auth/Reset.vue`)
  },
  // Pages that are not in the dashboard
  {
    path: '/',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/views/LoginView.vue`),
    children: [
      {
        path: '',
        component: () => import(`@/components/auth/Login.vue`)
      },
      {
        path: 'forgot',
        component: () => import(`@/components/auth/Forgot.vue`)
      }
    ]
  },
  // Pages that belong to the dashboard
  {
    path: '/dashboard',
    meta: {
      name: 'Dashboard',
      requiresAuth: true
    },
    component: () => import(`@/views/DashboardView.vue`),
    children: [
      // Insert child pages here
      {
        path: "",
        meta: {
          name: "Announcements",
        },
        component: () => import(`@/components/dashboard/announcement.vue`)
      },
      {
        path: "announcement",
        meta: {
          name: "Announcement",
        },
        component: () => import(`@/components/dashboard/announcement.vue`)
      },   
      {
        path: "discussion",
        meta: {
          name: "Discussion",
        },
        component: () => import(`@/components/dashboard/discussion.vue`)
      },        
      {
        path: "course",
        meta: {
          name: "Course Material",
        },
        component: () => import(`@/components/dashboard/course.vue`)
      },  
      {
        path: "lecture",
        meta: {
          name: "Live Lecture",
        },
        component: () => import(`@/components/dashboard/lecture.vue`)
      },
      {
        path: "user",
        meta: {
          name: "Manager User",
        },
        component: () => import(`@/components/dashboard/user.vue`)
      },
      {
        path: "post",
        meta: {
          name: "Manager Posts",
        },
        component: () => import(`@/components/dashboard/post.vue`)
      },                  
    ]
  }
]
