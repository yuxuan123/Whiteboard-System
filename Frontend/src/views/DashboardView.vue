<template>
  <v-content>
    <v-app id="dashboard-view">
      <core-drawer />
      <v-fade-transition mode="out-in">
        <router-view />
        <!-- style="margin-top:-120px !important;" -->
      </v-fade-transition>
    </v-app>
  </v-content>
</template>

<script>
export default {
  name: "DashboardView",
  metaInfo() {
    return {
      title: "Whiteboard"
    };
  }
};
</script>

<style>
</style>
