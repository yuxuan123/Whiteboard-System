<template>
  <v-container
    fluid
    grid-list-xl
    fill-height
  >
    <v-layout justify-center>
      <v-flex xs12>
        <material-card color="general">
          <div slot="header">
            <div class="title font-weight-light mb-2">
              Announcements
            </div>
            <div class="category">
              Check your tasks here!
            </div>
          </div>

          <v-card-text>
            <v-layout
              row
              wrap
            >
              <v-flex
                md12
                sm12
              >
                <h3 class="title">
                  Current Announcements
                </h3>
                <material-notification
                  v-for="item in notifications"
                  :key="item.id"
                  class="mb-2 test"
                  color="#ffffff"
                  icon="mdi-tag"
                >
                  {{ item.info }}
                </material-notification>
              </v-flex>
            </v-layout>
          </v-card-text>
        </material-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  data: () => ({
    notifications: [{ id: "1", info: "ac" }, { id:"2", info: "abc" }, { id: "3", info: "acbc" }]
  })
};
</script>

<style>
.text{
  color: red !important;
  border:0 !important;
}
</style>
