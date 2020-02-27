<template>
  <v-toolbar
    id="core-toolbar"
    flat
    color="transparent"
  >
    <div class="v-toolbar-title">
      <v-toolbar-title class="font-weight-light text-general">
        <v-btn
          v-if="responsive"
          class="default v-btn--simple"
          icon
          @click.stop="onClickBtn"
        >
          <v-icon>mdi-view-list</v-icon>
        </v-btn>
      </v-toolbar-title>
    </div>
    <v-spacer />
  </v-toolbar>
</template>

<script>
import { mapMutations, mapGetters } from "vuex";

export default {
  data: () => ({
    title: "Welcome to Whiteboard!",
    responsive: false,
    responsiveInput: false
  }),
  computed: {
    ...mapGetters(["authorized"])
  },
  watch: {
    $route(val) {
      this.title = val.meta.name;
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
    onClickBtn() {
      this.setDrawer(!this.$store.state.app.drawer);
    },
    onResponsiveInverted() {
      if (window.innerWidth < 991) {
        this.responsive = true;
        this.responsiveInput = false;
      } else {
        this.responsive = false;
        this.responsiveInput = true;
      }
    }
  }
};
</script>

<style>
#core-toolbar a {
  text-decoration: none;
}
</style>
