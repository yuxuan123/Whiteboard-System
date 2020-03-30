<template>
  <v-container
    fluid
    grid-list-xl
    fill-height
    class="discussion-container"
  >
    <v-layout
      row
      wrap
    >
      <!-- Mailbox page -->
      <v-flex
        xs12
        sm12
        md5
        class="pa-0"
      >
        <v-card height="100%">
          <v-list
            style="max-height: 100vh;"
            class="scroll-y"
            three-line
          >
            <v-layout style="max-width:100%;">
              <v-flex
                sm12
                md5
              >
                <v-text-field
                  v-model.lazy="search"
                  class="pr-0 pt-3 pb-0 pl-3"
                  append-icon="search"
                  label="Search"
                  single-line
                  hide-details
                />
              </v-flex>

              <v-flex
                sm12
                md7
              >
                <v-select
                  v-model="searchCourseId"
                  :items="courses"
                  item-text="courseName"
                  item-value="courseId"
                  label="Courses"
                  class="pr-3 pt-3 pb-0 pl-0"
                  dense
                  @input="searchByCourse"
                />
              </v-flex>
            </v-layout>

            <v-btn
              color="general"
              class="ml-3 mt-0"
              @click="page = 'create-discussion'"
            >
              New Discussion
            </v-btn>

            <template v-if="discussions.length == 0">
              <p class="active text-xs-center mt-3">
                No discussions to display...
              </p>
            </template>

            <template v-for="(item, index) in discussions">
              <v-list-tile
                :key="item.postId"
                avatar
                class="discussionList"
                @click="displayDiscussion(item)"
              >
                <v-list-tile-avatar class="pt-3">
                  <img src="https://bit.ly/2VyYYzy">
                </v-list-tile-avatar>

                <v-list-tile-content>
                  <v-list-tile-title> {{ item.title }}</v-list-tile-title>
                  <v-list-tile-sub-title>
                    {{ stripHTML(item.description) }}
                  </v-list-tile-sub-title>
                </v-list-tile-content>

                <v-list-tile-action>
                  <v-list-tile-action-text>
                    {{ moment.utc(item.createdOn).fromNow() }}
                  </v-list-tile-action-text>
                </v-list-tile-action>
              </v-list-tile>
              <v-divider
                :key="index"
                style="max-width:100%;"
              />
            </template>
          </v-list>
        </v-card>
      </v-flex>
      <!-- Default view -->
      <v-flex
        v-if="page == 'default'"
        xs12
        sm12
        md7
        class="discussion-page-padding"
      >
        <v-card height="100%">
          <v-card-text>
            <v-layout
              column
              class="mt-5"
            >
              <v-img
                class="text-center"
                :src="discussImg"
                height="400"
                contain
              />
              <h3
                class="headline font-italic font-weight-light active text-xs-center"
              >
                Have a question? Start a discussion now!
              </h3>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
      <!-- Create Discussion view -->
      <v-flex
        v-if="page == 'create-discussion'"
        xs12
        sm12
        md7
        class="discussion-page-padding"
      >
        <v-card height="100%">
          <v-card-title class="discussion-title-color">
            <span
              class="headline white--text font-weight-thin pa-2"
            >New Discussion</span>

            <v-spacer />
            <v-btn
              icon
              @click="page = 'default'"
            >
              <v-icon color="white">
                mdi-close
              </v-icon>
            </v-btn>
          </v-card-title>

          <v-divider />
          <v-card-text>
            <v-form ref="createform">
              <v-container>
                <v-layout
                  row
                  wrap
                >
                  <v-flex
                    xs12
                    sm12
                    md12
                  >
                    <v-text-field
                      v-model="newDiscussion.title"
                      label="Title"
                      required
                    />
                  </v-flex>
                  <v-flex
                    xs12
                    sm6
                    md6
                  >
                    <v-select
                      v-model="newDiscussion.courseId"
                      :items="courses"
                      item-text="courseName"
                      item-value="courseId"
                      label="Course"
                      @input="changeCourseFolders()"
                    />
                  </v-flex>
                  <v-flex
                    xs12
                    sm6
                    md6
                  >
                    <v-select
                      v-model="newDiscussion.courseFolderId"
                      :items="selectedCourseFolders"
                      item-text="name"
                      item-value="courseFolderId"
                      label="Course Folder"
                    />
                  </v-flex>
                  <v-flex
                    xs12
                    sm12
                    md12
                  >
                    <ckeditor
                      v-model="newDiscussion.description"
                      :editor="editor"
                    />
                  </v-flex>
                </v-layout>
              </v-container>
            </v-form>
          </v-card-text>

          <v-card-actions>
            <v-spacer />
            <v-btn
              color="warning"
              @click="newDiscussion = {}"
            >
              Clear
            </v-btn>
            <v-btn
              color="blue"
              @click="createDiscussion"
            >
              Post
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
      <!-- Show Discussion view -->
      <v-flex
        v-if="page == 'view-discussion'"
        xs12
        sm12
        md7
        class="discussion-page-padding"
      >
        <v-card style="height: 100%;">
          <v-list
            class="scroll-y discussion-thread"
            three-line
          >
            <v-card-title class="discussion-title-fixed">
              <span
                class="headline white--text font-weight-thin pa-2"
              >View Discussion</span>

              <v-spacer />
              <v-btn
                icon
                @click="page = 'default'"
              >
                <v-icon style="color:white;">
                  mdi-close
                </v-icon>
              </v-btn>
            </v-card-title>

            <v-divider />
            <v-card-text>
              <v-layout
                row
                wrap
                align-center
              >
                <v-flex
                  xs12
                  md12
                >
                  <v-card
                    class="pa-3"
                    hover
                  >
                    <v-container fluid>
                      <v-layout column>
                        <v-flex>
                          <span class="headline font-weight-light pt-3">{{
                            selectedDiscussion.title
                          }}</span>
                          <v-chip
                            outline
                            color="primary"
                            style="margin-top:-3px;margin-left:10px;"
                          >
                            {{ selectedDiscussion.courseName }}
                          </v-chip>
                          <v-chip
                            outline
                            color="secondary"
                            style="margin-top:-3px;margin-left:10px;"
                          >
                            {{ selectedDiscussion.courseFolderName }}
                          </v-chip>
                        </v-flex>
                        <span class="subtitle-1 font-weight-light pa-1">{{
                          selectedDiscussion.datetime
                        }}</span>
                      </v-layout>
                    </v-container>
                    <v-card-text>
                      {{ stripHTML(selectedDiscussion.description) }}
                    </v-card-text>
                    <v-card-actions class="pa-0 ml-2">
                      <v-chip
                        small
                        color="secondary"
                        class="white--text"
                      >
                        From:
                        {{ fetchNameFromUserId(selectedDiscussion.createdBy) }}
                      </v-chip>
                      <v-btn
                        icon
                        class="light-blue--text"
                      >
                        <v-icon small>
                          mdi-twitter
                        </v-icon>
                      </v-btn>
                      <v-btn
                        icon
                        class="blue--text text--darken-4"
                      >
                        <v-icon small>
                          mdi-facebook
                        </v-icon>
                      </v-btn>
                      <v-spacer />
                      <v-btn
                        small
                        color="info"
                        @click="commentBox = !commentBox"
                      >
                        Add comment
                      </v-btn>
                    </v-card-actions>
                    <v-flex v-if="commentBox">
                      <v-textarea
                        v-model="newComment"
                        append-outer-icon="send"
                        class="mx-2"
                        label="Message to send"
                        rows="2"
                        @click:append-outer="addComment(newComment)"
                      />
                    </v-flex>
                  </v-card>
                  <template v-for="(item, index) in discussionPosts">
                    <v-layout
                      :key="index"
                      row
                      justify-end
                    >
                      <v-flex
                        xs11
                        md11
                      >
                        <div class="line" />
                        <v-card
                          class="pa-3"
                          hover
                        >
                          <v-card-text>{{ item.description }}</v-card-text>

                          <v-card-actions class="pa-0 ml-2">
                            <v-chip
                              small
                              color="secondary"
                              class="white--text"
                            >
                              From: {{ fetchNameFromUserId(item.createdBy) }}
                            </v-chip>
                            <v-chip
                              small
                              color="blue"
                              class="white--text"
                            >
                              {{ moment.utc(item.createdOn).fromNow() }}
                            </v-chip>
                            <v-btn
                              icon
                              class="light-blue--text"
                            >
                              <v-icon small>
                                mdi-twitter
                              </v-icon>
                            </v-btn>
                            <v-btn
                              icon
                              class="blue--text text--darken-4"
                            >
                              <v-icon small>
                                mdi-facebook
                              </v-icon>
                            </v-btn>
                          </v-card-actions>
                        </v-card>
                      </v-flex>
                    </v-layout>
                  </template>
                </v-flex>
              </v-layout>
            </v-card-text>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
    <v-snackbar
      v-model="snackbar"
      :color="color"
      :top="true"
    >
      {{ message }}
      <v-btn
        dark
        flat
        @click="snackbar = false"
      >
        Close
      </v-btn>
    </v-snackbar>
  </v-container>
</template>

<script>
import moment from "moment";
var striptags = require("striptags");
import ClassicEditor from "@ckeditor/ckeditor5-build-classic";

export default {
  data() {
    return {
      editor: ClassicEditor,
      editorConfig: {
        toolbar: {
          items: [
            "heading",
            "|",
            "bold",
            "italic",
            "link",
            "bulletedList",
            "numberedList",
            "undo",
            "redo"
          ]
        }
      },
      search: "",
      searchCourseId: "",
      page: "default",
      commentBox: false,
      newComment: "",
      users: [],
      courses: [],
      courseFolders: [],
      selectedCourseFolders: [],
      discussionTypes: ["Lecture", "Tutorial", "Assignment", "Exam"],
      discussImg: require("@/assets/default/discussion.png"),
      discussions: [],
      discussionPosts: [],
      discussionPostsUser: [],
      newDiscussion: {},
      selectedDiscussion: {},
      snackbar: false,
      color: "general",
      message: "",
      username: ""
    };
  },
  watch: {
    search: {
      handler(input) {
        this.searchPosts(input);
      }
    }
  },
  created() {
    this.getAllUsers();
    this.fetchUserCourses();
    this.fetchAllCourseFolders();
    this.fetchUserRelatedDiscussions();
  },
  methods: {
    stripHTML(item) {
      let text = striptags(item);
      return text;
    },
    searchPosts(input) {
      if (input.length > 3 || input.length == 0) {
        this.fetchUserRelatedDiscussions();
      }
    },
    searchByCourse() {
      this.fetchUserRelatedDiscussions();
    },
    changeCourseFolders() {
      this.$store
        .dispatch("GETCOURSEFOLDERS", this.newDiscussion.courseId)
        .then(response => {
          this.selectedCourseFolders = response.data;
        });
    },
    getAllUsers() {
      this.$store
        .dispatch("GETALLUSERS")
        .then(response => {
          this.users = response.data.userDtos;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
    },
    fetchUserCourses() {
      //Get the list of courses first
      //For Dropdownlist
      //get userId
      let userId = $cookies.get("userid");
      this.$store
        .dispatch("GETUSERCOURSES", userId)
        .then(response => {
          this.courses = response.data;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
    },
    fetchAllCourseFolders() {
      this.$store
        .dispatch("GETALLCOURSEFOLDERS")
        .then(response => {
          this.courseFolders = response.data;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
    },
    fetchUserRelatedDiscussions() {
      //Get userid
      let userId = $cookies.get("userid");
      var item = {
        userId: userId,
        courseId: this.searchCourseId,
        keyword: this.search,
        OrderBy: "createdOn"
      };
      this.$store.dispatch("GETUSERCOURSEDISCUSSIONS", item).then(response => {
        this.discussions = response.data;
      });
    },
    fetchNameFromUserId(userId) {
      //Temp solution
      //Get all users and save to array
      //Search and return by id
      var userIndex = this.users.findIndex(x => x.userId === userId);
      if (userIndex != -1) {
        return this.users[userIndex].userName;
      }
    },
    displayDiscussion(discussionPost) {
      this.page = "view-discussion";
      this.selectedDiscussion = {};
      this.selectedDiscussion.postId = discussionPost.postId;
      //Convert course and course folder index to name
      var courseIndex = this.courses.findIndex(
        x => x.courseId === discussionPost.courseId
      );
      var courseFolderIndex = this.courseFolders.findIndex(
        x => x.courseFolderId === discussionPost.courseFolderId[0]
      );
      if (courseIndex != -1) {
        this.selectedDiscussion.courseName = this.courses[
          courseIndex
        ].courseName;
      } else {
        this.selectedDiscussion.courseName = "N/A";
      }

      if (courseFolderIndex != -1) {
        this.selectedDiscussion.courseFolderName = this.courseFolders[
          courseFolderIndex
        ].name;
      } else {
        this.selectedDiscussion.courseFolderName = "No folder";
      }
      this.selectedDiscussion.title = discussionPost.title;
      this.selectedDiscussion.description = discussionPost.description;
      this.selectedDiscussion.datetime = moment(
        discussionPost.createdOn
      ).format("DD/MM/YY HH:mm:ss");
      this.selectedDiscussion.createdBy = discussionPost.createdBy;
      this.$store
        .dispatch("GETPOSTREPLIES", discussionPost.postId)
        .then(response => {
          this.discussionPosts = response.data;
        });
      //Display Discussion Posts
      //Future enhancements - differentate posts from lecturer and student
      /* var lecturerReplies = discussionPost.lecturerReply;
      var studentReplies = discussionPost.studentReply;
      var totalReplies = lecturerReplies.concat(studentReplies); */
    },
    addComment(message) {
      var newReply = {};
      newReply.postId = this.selectedDiscussion.postId;
      newReply.description = message;
      newReply.role = $cookies.get("role");
      newReply.userName = $cookies.get("username");
      newReply.isEdited = true;
      newReply.createdOn = moment(new Date());
      newReply.createdBy = $cookies.get("userid");
      this.$store.dispatch("CREATEPOSTREPLY", newReply).then(response => {
        this.discussionPosts.push(newReply);
        this.newComment = "";
      });
    },
    createDiscussion() {
      //Add all the required fields before passing it over
      this.newDiscussion.createdBy = $cookies.get("userid");
      this.newDiscussion.createdOn = moment(new Date());
      this.newDiscussion.userName = $cookies.get("username");
      var id = this.newDiscussion.courseFolderId;

      this.newDiscussion.courseFolderId = [];
      this.newDiscussion.courseFolderId.push(id);
      var item = this.newDiscussion;
      delete item.courseName;
      delete item.courseFolderName;

      this.$store
        .dispatch("CREATEDISCUSSION", item)
        .then(response => {
          this.discussions.push(item);
          //Clear selectedCourseFolder & newDiscussion
          this.selectedCourseFolders = [];
          this.newDiscussion = {};
          this.page = "default";
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Create Error, Please try again later";
        });
    }
  }
};
</script>
<style>
@import url("../../styles/custom/discussion.css");

.discussionList a{
  padding-top: 20px !important;
  padding-bottom: 20px !important;
  padding-left: 30px !important;
  padding-right: 30px !important;
}
</style>
