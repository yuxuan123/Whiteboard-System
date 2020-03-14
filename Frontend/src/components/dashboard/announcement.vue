<template>
  <v-container fluid grid-list-xl fill-height>
    <v-flex justify-center>
      <v-flex xs12>
        <material-notification
          class="mb-3"
          color="general"
          dismissible
          icon="mdi-school"
        >
          Live Lectures ongoing now!!
        </material-notification>

        <material-card color="general">
          <div slot="header">
            <div class="title font-weight-light mb-2">
              Announcements
            </div>
            <div class="category">
              Check your tasks here!
            </div>
          </div>

          <v-layout row wrap>
            <v-list three-line style="width:100%;">
              <template v-for="(announcement, index) in announcements">
                <v-flex :key="announcement.contentId" class="p-2">
                  <v-list-tile avatar>
                    <v-list-tile-avatar>
                      <img
                        src="https://image.flaticon.com/icons/svg/326/326031.svg"
                      />
                    </v-list-tile-avatar>

                    <v-list-tile-content>
                      <div class="announcementTitle">
                        {{ getCourseInfo(announcement.courseId) }}
                      </div>
                      <v-list-tile-title class="announcementHeader">
                        {{ announcement.title }} -
                        {{
                          moment(announcement.datetime).format(
                            "MMMM Do YYYY, h:mm:ss a"
                          )
                        }}
                      </v-list-tile-title>
                      <v-list-tile-sub-title class="my-2 announcementContent">
                        <template v-if="announcement.description == ''">
                          No content here :|
                        </template>
                        <template else>
                          {{ announcement.description }}
                        </template>
                      </v-list-tile-sub-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </v-flex>
                <v-divider :key="index" style="max-width:100%;"></v-divider>
              </template>
            </v-list>
          </v-layout>
        </material-card>
      </v-flex>
    </v-flex>
  </v-container>
</template>
<script>
import moment from "moment";
export default {
  data() {
    return {
      announcements: [],
      courses: []
    };
  },
  created() {
    //Get list of courses first
    this.$store.dispatch("GETALLCOURSES").then(response => {
      this.courses = response.data;
    });
    //Get userid
    let userId = $cookies.get("userid");
    this.$store.dispatch("GETUSERCONTENT", userId).then(response => {
      // Check if type is announcement first.
      for(var i = 0; i < response.data.length; i++){
        if(response.data[i].type == "announcement"){
          this.announcements.push(response.data[i]);
        }
      }
    });
  },
  methods: {
    getCourseInfo(courseId) {
      var courseIndex = this.courses.findIndex(x => x.courseId === courseId);
      var text = "Announcements";
      if (courseIndex != "-1") {
        text =
          "Announcements -> " +
          this.courses[courseIndex].courseCode +
          " " +
          this.courses[courseIndex].courseName;
      }
      return text;
    }
  }
};
</script>

<style>
.announcementTitle {
  color: #47444a;
  font-size: 14px;
  margin-top: 10px;
}

.announcementHeader {
  font-size: 18px;
  font-weight: 400;
}

.announcementContent {
  font-size: 16px;
}

.v-alert .v-icon{
  color:white !important;
}
</style>
