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
                md6
              >
                <v-text-field
                  class="pr-0 pt-3 pb-0 pl-3"
                  append-icon="search"
                  label="Search"
                  single-line
                  hide-details
                />
              </v-flex>

              <v-flex
                sm12
                md6
              >
                <v-select
                  :items="discussionTypes"
                  class="pr-3 pt-3 pb-0 pl-0"
                  label="Courses"
                  dense
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
                @click="displayDiscussion(item)"
              >
                <v-list-tile-avatar>
                  <img src="https://bit.ly/2VyYYzy">
                </v-list-tile-avatar>

                <v-list-tile-content>
                  <v-list-tile-title> {{ item.title }}</v-list-tile-title>
                  <v-list-tile-sub-title>
                    {{
                      stripHTML(item.description)
                    }}
                  </v-list-tile-sub-title>
                </v-list-tile-content>
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
                      label="Title"
                      required
                    />
                  </v-flex>
                  <v-flex
                    xs12
                    sm12
                    md12
                  >
                    <v-select
                      :items="discussionTypes"
                      label="Category"
                    />
                  </v-flex>
                  <v-flex
                    xs12
                    sm12
                    md12
                  >
                    <ckeditor :editor="editor" />
                  </v-flex>
                </v-layout>
              </v-container>
            </v-form>
          </v-card-text>

          <v-card-actions>
            <v-spacer />
            <v-btn color="warning">
              Clear
            </v-btn>
            <v-btn color="blue">
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
                        <span class="headline font-weight-light pt-3">{{
                          selectedDiscussion.title
                        }}</span>
                        <span class="subtitle-1 font-weight-light pa-1">{{
                          selectedDiscussion.datetime
                        }}</span>
                      </v-layout>
                    </v-container>
                    <v-card-text>{{ selectedDiscussion.body }}</v-card-text>
                    <v-card-actions class="pa-0 ml-2">
                      <v-chip
                        small
                        color="secondary"
                        class="white--text"
                      >
                        From: {{ selectedDiscussion.createdby }}
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
                          <v-card-text>{{ item.body }}</v-card-text>
                          <v-card-actions class="pa-0 ml-2">
                            <v-chip
                              small
                              color="secondary"
                              class="white--text"
                            >
                              From: {{ item.createdby }}
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
  </v-container>
</template>

<script>
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
      page: "default",
      commentBox: false,
      newComment: "",
      discussionTypes: ["Lecture", "Tutorial", "Assignment", "Exam"],
      discussImg: require("@/assets/default/discussion.png"),
      discussions: [],
      discussionPosts: [
        {
          id: "dp1",
          body: "Here's comment number 1. Can you see it?",
          createdby: "Alan"
        },
        {
          id: "dp2",
          body: "Here's comment number 2. Can you see it?",
          createdby: "Bill"
        },
        {
          id: "dp3",
          body: "Here's comment number 3. Can you see it?",
          createdby: "Charlie"
        },
        {
          id: "dp4",
          body: "Here's comment number 4. Can you see it?",
          createdby: "David"
        },
        {
          id: "dp5",
          body: "Here's comment number 5. Can you see it?",
          createdby: "Evan"
        }
      ],
      selectedDiscussion: {}
    };
  },
  created() {
    this.fetchUserRelatedDiscussions();
  },
  methods: {
    stripHTML(item) {
      let text = striptags(item);
      return text;
    },
    fetchUserRelatedDiscussions() {
      //Get userid
      let userId = $cookies.get("userid");
      this.$store
        .dispatch("GETUSERCOURSEDISCUSSIONS", userId)
        .then(response => {
          console.log(response);
          this.discussions = response.data;
        });
    },
    displayDiscussion(discussionPost) {
      this.page = "view-discussion";
      this.selectedDiscussion = {};
      this.selectedDiscussion.title = discussionPost.title;
      this.selectedDiscussion.body = discussionPost.body;
      this.selectedDiscussion.datetime = "01/03/20 20:30:00pm";
      this.selectedDiscussion.createdby = discussionPost.createdby;

      //Fetch related posts
      //displayDiscussionPost(discussionPost.id);
    },
    displayDiscussionPost(id) {
      //Fetch discussion posts by id
    },
    addComment(message) {
      var item = {
        id: "dp1",
        body: message,
        createdby: "Alan"
      };
      this.newComment = "";
      this.discussionPosts.push(item);
    }
  }
};
</script>
<style>
@import url("../../styles/custom/discussion.css");
</style>
