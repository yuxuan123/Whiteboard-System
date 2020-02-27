<template>
  <v-app
    id="404"
    dark
  >
    <v-container
      fluid
      fill-height
    >
      <v-layout
        align-center
        justify-center
      >
        <div class="text-md-center">
          <h1 class="errorTitle">
            404
          </h1>
          <h2 class="my-3 headline">
            Sorry, page not found
          </h2>
          <div>
            <v-btn
              color="primary"
              @click="goHome"
            >
              Go Home
            </v-btn>
          </div>
        </div>
      </v-layout>
    </v-container>
  </v-app>
</template>

<script>
export default {
  methods: {
    goHome() {
      this.$router.push({ path: "/" });
    }
  }
};
</script>
<style scoped lang="css">
.errorTitle {
  font-size: 150px !important;
  line-height: 150px !important;
  font-weight: 100 !important;
  color: #ffffff;
}
</style>
