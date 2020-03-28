/**
 * Define all of your application routes here
 * for more information on routes, see the
 * official documentation https://router.vuejs.org/en/
 */
import store from '../store'
import Cookies from 'js-cookie'

//Check from cookies if the role is admin
//If yes, enable routing to the page
const checkAdmin = (to, from, next) => {
  var role = Cookies.get('role');
  if (role === 'admin') {
    next()
  }
  else {
    next('/')
  }
}

export default [
  {
    path: '*',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/error/NotFound.vue`)
  },
  {
    path: '/resetpassword/:userId',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/auth/Reset.vue`)
  },
  {
    path: '/activate/:userId',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/components/auth/Reset.vue`)
  },
  // Pages that are not in the dashboard
  {
    path: '/',
    component: () =>
      import(/* webpackChunkName: "routes" */ `@/views/LoginView.vue`),
    // redirect if already signed in
    beforeEnter: (to, from, next) => {
      if (Cookies.get('authenticated')) {
        next("/dashboard");
      } else {
        next();
      }
    },
    children: [
      {
        path: '',
        component: () => import(`@/components/auth/Login.vue`),
      },
      {
        path: 'login',
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
          name: "Landing",
          requiresAuth: true
        },
        component: () => import(`@/components/dashboard/landing.vue`)
      },
      {
        path: "announcement",
        meta: {
          name: "Announcement",
          requiresAuth: true
        },
        component: () => import(`@/components/dashboard/announcement.vue`)
      },
      {
        path: "discussion",
        meta: {
          name: "Discussion",
          requiresAuth: true
        },
        component: () => import(`@/components/dashboard/discussion.vue`)
      },
      {
        path: "course",
        meta: {
          name: "Course Material",
          requiresAuth: true
        },
        component: () => import(`@/components/dashboard/course.vue`)
      },
      {
        path: "lecture",
        meta: {
          name: "Live Lecture",
          requiresAuth: true
        },
        component: () => import(`@/components/dashboard/lecture.vue`)
      },
      {
        path: "user",
        meta: {
          name: "Manage User",
          requiresAuth: true
        },
        beforeEnter: checkAdmin,
        component: () => import(`@/components/dashboard/user.vue`)
      },
      {
        path: "post",
        meta: {
          name: "Manage Posts",
          requiresAuth: true
        },
        beforeEnter: checkAdmin,
        component: () => import(`@/components/dashboard/post.vue`)
      },
    ]
  }
]
