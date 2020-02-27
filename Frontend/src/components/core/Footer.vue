<template>
  <v-footer
    id="core-footer"
    absolute
    height="82"
  >
    <div class="footer-items">
      <span
        v-for="link in links"
        :key="link.name"
      >
        <a
          :href="link.Link"
          class="tertiary--text footer-links"
        >{{
          link.name
        }}</a>
      </span>
    </div>
    <v-spacer />
    <span class="font-weight-light copyright">
      &copy; {{ new Date().getFullYear() }}, made with
      <v-icon size="18">mdi-heart</v-icon> by
      <a href="https://www.whiteboard.com" /> Whiteboard
    </span>
  </v-footer>
</template>

<script>
export default {
  data: () => ({
    links: [
      { name: "Home", Link: "#" },
      { name: "Whiteboard", Link: "#" },
      { name: "About Us", Link: "#" },
      { name: "Licence", Link: "#" }
    ]
  })
};
</script>

<style>
#core-footer {
  z-index: 0;
}
</style>
