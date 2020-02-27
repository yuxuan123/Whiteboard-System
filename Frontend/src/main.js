// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from "vue";

// Components
import "./components";

// Sync router with store
import { sync } from "vuex-router-sync";

// Application imports
import App from "./App";
import router from "@/router";
import store from "@/store";
import Vuetify from "vuetify";
import theme from "./styles/theme";
import "vuetify/dist/vuetify.min.css";
import "@mdi/font/css/materialdesignicons.css";
import "material-design-icons-iconfont/dist/material-design-icons.css";
import "viewerjs/dist/viewer.css";
import axios from "axios";
import VueAxios from "vue-axios";
import moment from "moment";
import CKEditor from "@ckeditor/ckeditor5-vue";
import Viewer from "v-viewer";
import VueCookies from 'vue-cookies'
import VueParticles from 'vue-particles'

// Sync store with router
sync(store, router);

// Use plugins by calling the Vue.use() global method
// This have to be done before calling new Vue()
Vue.prototype.moment = moment;
Vue.use(CKEditor);
Vue.use(VueAxios, axios);
Vue.use(VueCookies)
Vue.use(Viewer);
Vue.use(VueParticles);
Vue.use(Vuetify, {
  iconfont: "mdi",
  theme
});

// Disable annoying production mode warning 
// ("You are running Vue in devmode") in Console
Vue.config.productionTip = false;

// Creating a new vue instance
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
