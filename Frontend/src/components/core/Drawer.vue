<template>
  <v-navigation-drawer
    id="app-drawer"
    v-model="inputValue"
    app
    dark
    floating
    persistent
    mobile-break-point="991"
    width="260"
  >
    <v-img
      :src="image"
      height="100%"
    >
      <v-layout
        class="fill-height"
        tag="v-list"
        column
      >
        <router-link to="/">
          <v-img
            :src="logo"
            height="120"
            style="margin-left: -30px;margin-top:0px"
            contain
          />
        </router-link>
        <v-divider style="margin-top:-33px;margin-bottom:0px;" />
        <v-list-tile
          v-for="(link, i) in links"
          :key="i"
          :to="link.to"
          :active-class="color"
          avatar
          class="v-list-item"
          style="margin: 10px 15px 0 !important"
        >
          <v-list-tile-action>
            <v-icon>{{ link.icon }}</v-icon>
          </v-list-tile-action>
          <v-list-tile-title v-text="link.text" />
        </v-list-tile>
        <v-divider style="margin-top:10px;margin-bottom:0px" />
        <v-list-tile
          class="v-list-item"
          style="margin: 10px 15px 0 !important;"
          @click="logout"
        >
          <v-list-tile-action>
            <v-icon>mdi-power</v-icon>
          </v-list-tile-action>
          <v-list-tile-title>Logout</v-list-tile-title>
        </v-list-tile>
      </v-layout>
    </v-img>
  </v-navigation-drawer>
</template>

<script>
// Utilities
import { mapMutations, mapState } from "vuex";

export default {
  data: () => ({
    logo: require("@/assets/default/icon.png"),
    links: [
      {
        to: "/dashboard/announcement",
        icon: "mdi-bell-ring",
        text: "All Announcement",
        name: "Announcement"
      },
      {
        to: "/dashboard/discussion",
        icon: "mdi-wechat",
        text: "Discussions",
        name: "Discussions"
      },
      {
        to: "/dashboard/course",
        icon: "mdi-library-books",
        text: "Course Materials",
        name: "Course Materials"
      },
      {
        to: "/dashboard/lecture",
        icon: "mdi-video",
        text: "Live Lectures",
        name: "Live Lectures"
      },
      {
        to: "/dashboard/user",
        icon: "mdi-file-account",
        text: "Manage Users",
        name: "Manage Users"
      },
      {
        to: "/dashboard/post",
        icon: "mdi-file-check",
        text: "Manage Posts",
        name: "Manage Posts"
      }
    ],
    responsive: false
  }),
  computed: {
    ...mapState("app", ["image", "color"]),
    inputValue: {
      get() {
        return this.$store.state.app.drawer;
      },
      set(val) {
        this.setDrawer(val);
      }
    },
    items() {
      return this.$t("Layout.View.items");
    }
  },
  created: function() {
    var role = $cookies.get("role");
    if (role == "student" || role == "lecturer") {
      // Hide sidebar tabs that is not related to user
      var userIndex = this.links.findIndex(x => x.name === "Manage Users");
      this.links.splice(userIndex, 1);
      var discountIndex = this.links.findIndex(x => x.name === "Manage Posts");
      this.links.splice(discountIndex, 1);
    }
    if (role == "admin") {
      // Remove all tabs before Manage Users
      var userIndex = this.links.findIndex(x => x.name === "Manage Users");
      this.links = this.links.slice(userIndex, this.links.length);
    }
  },
  mounted() {
    this.onResponsiveInverted();
    window.addEventListener("resize", this.onResponsiveInverted);
  },
  beforeDestroy() {
    window.removeEventListener("resize", this.onResponsiveInverted);
  },
  methods: {
    ...mapMutations("app", ["setDrawer", "toggleDrawer"]),
    onResponsiveInverted() {
      if (window.innerWidth < 991) {
        this.responsive = true;
      } else {
        this.responsive = false;
      }
    },
    logout: function() {
      this.$store.dispatch("LOGOUT").then(() => {
        this.$router.push("/");
      });
    }
  }
};
</script>

<style lang="scss">
#app-drawer {
  .v-list__tile {
    border-radius: 4px;
  }

  .v-image__image--contain {
    top: 30px;
    height: 30px;
    width: 220px;
    left: 50px;
  }

  .container {
    padding-top: 0px !important;
  }
}
.container {
  padding-top: 0px !important;
}
</style>
