<template>
  <v-container
    fill-height
    fluid
    grid-list-xl
  >
    <v-layout
      align-center
      justify-center
      wrap
    >
      <v-flex md12>
        <div>
          <material-card
            color="general"
            title="Admin Dashboard"
            text="Manage Discussion Posts"
          >
            <v-spacer />
            <br>
            <v-dialog
              v-model="discussionDialog"
              max-width="1000px"
            >
              <v-card>
                <v-card-title>
                  <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                  <v-container
                    class="pt-0"
                    grid-list-md
                  >
                    <v-layout wrap>
                      <v-flex
                        xs12
                        sm12
                        md12
                      >
                        <v-text-field
                          v-model="viewingDiscussion.title"
                          label="Title"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm6
                        md6
                      >
                        <v-text-field
                          v-model="viewingDiscussion.courseName"
                          label="Course"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm6
                        md6
                      >
                        <v-text-field
                          v-model="viewingDiscussion.courseFolderName"
                          label="Course Folder"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm6
                        md6
                      >
                        <v-text-field
                          v-model="viewingDiscussion.createdOn"
                          label="Created On"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm6
                        md6
                      >
                        <v-text-field
                          v-model="viewingDiscussion.userName"
                          label="Created By"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm12
                        md12
                        class="viewEditor"
                        style="margin-top:10px;"
                      >
                        <ckeditor
                          v-model="viewingDiscussion.description"
                          disabled
                          :editor="editor"
                        />
                      </v-flex>
                    </v-layout>
                  </v-container>
                  <v-card-actions>
                    <v-spacer />
                    <v-btn
                      color="blue darken-1"
                      flat
                      @click="closeviewingDiscussion"
                    >
                      Close
                    </v-btn>
                  </v-card-actions>
                </v-card-text>
              </v-card>
            </v-dialog>
            <v-dialog
              v-model="dialog"
              max-width="1000px"
            >
              <template v-slot:activator="{ on }">
                <v-btn
                  color="general"
                  dark
                  v-on="on"
                >
                  New Discussion
                </v-btn>
              </template>

              <v-card>
                <v-card-text>
                  <v-card-title>
                    <span class="headline">{{ formTitle }}</span>
                  </v-card-title>
                  <v-form ref="createform">
                    <v-container
                      class="pt-0"
                      grid-list-md
                    >
                      <v-layout wrap>
                        <v-flex
                          xs12
                          sm12
                          md12
                        >
                          <v-text-field
                            v-model="editedDiscussion.title"
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
                            v-model="editedDiscussion.courseId"
                            :items="courses"
                            item-text="courseName"
                            item-value="courseId"
                            label="Course"
                          />
                        </v-flex>
                        <v-flex
                          xs12
                          sm6
                          md6
                        >
                          <v-select
                            v-model="editedDiscussion.courseFolderId"
                            :items="courseFolders"
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
                            v-model="editedDiscussion.description"
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
                    color="blue darken-1"
                    flat
                    @click="close"
                  >
                    Cancel
                  </v-btn>
                  <v-btn
                    v-if="!isViewing"
                    color="blue darken-1"
                    flat
                    @click="save"
                  >
                    Save
                  </v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>

            <v-flex
              xs12
              sm12
              md12
            >
              <v-text-field
                v-model.lazy="search"
                class="mb-2"
                append-icon="search"
                label="Search"
                single-line
                hide-details
              />
            </v-flex>

            <v-data-table
              :headers="headers"
              :items="DiscussionList"
              :pagination.sync="pagination"
              :rows-per-page-items="pagination.rowsPerPageItems"
              :total-items="pagination.totalItems"
              :loading="loading"
              class="elevation-1"
            >
              <!-- change table header background and text color(or other properties) -->
              <template
                slot="headerCell"
                slot-scope="{ header }"
              >
                <span
                  class="subheading font-weight-light text-general text--darken-3"
                  v-text="header.text"
                />
              </template>
              <template
                slot="items"
                slot-scope="props"
              >
                <td>{{ props.item.title }}</td>
                <td>{{ stripHTML(props.item.description) }}</td>
                <td>{{ props.item.courseName }}</td>
                <td>{{ props.item.courseFolderName }}</td>
                <td>{{ props.item.userName }}</td>
                <td>
                  {{ props.item.createdOn }}
                </td>
                <td>
                  <v-layout
                    align-center
                    justify-center
                  >
                    <v-card-actions>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="viewDiscussion(props.item)"
                      >
                        <v-icon
                          size="20"
                          color="blue lighten-1"
                        >
                          mdi-eye
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="editDiscussion(props.item)"
                      >
                        <v-icon
                          size="20"
                          color="blue lighten-1"
                        >
                          edit
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="deleteDiscussion(props.item)"
                      >
                        <v-icon
                          size="20"
                          color="blue lighten-1"
                        >
                          delete
                        </v-icon>
                      </v-btn>
                    </v-card-actions>
                  </v-layout>
                </td>
              </template>
            </v-data-table>
          </material-card>
        </div>
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
import ClassicEditor from "@ckeditor/ckeditor5-build-classic";
var striptags = require("striptags");

export default {
  data: () => ({
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
    loading: false,
    isViewing: false,
    discussionDialog: false,
    search: "",
    pagination: {
      search: "",
      page: 1,
      rowsPerPage: 5,
      sortBy: "title",
      descending: true,
      rowsPerPageItems: [5, 10, 15],
      totalItems: 0
    },
    DiscussionList: [],
    courses: [],
    courseFolders: [],
    dialog: false,
    headers: [
      {
        text: "Title",
        value: "title"
      },
      { text: "Description", value: "description" },
      {
        text: "Course Name",
        value: "courseName",
        filterable: false,
        sortable: false
      },
      {
        text: "Course Folder",
        value: "courseFolderName",
        filterable: false,
        sortable: false
      },
      {
        text: "Created By",
        value: "userName",
        filterable: false,
        sortable: false
      },
      {
        text: "Created On",
        value: "createdOn"
      },
      {
        text: "Actions",
        value: "actions",
        align: "center",
        filterable: false,
        sortable: false,
        width: 150
      }
    ],
    editedIndex: -1,
    editedDiscussion: {},
    viewingDiscussion: {},
    snackbar: false,
    color: "general",
    message: ""
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "New Discussion"
        : this.editedIndex === -2
        ? "View Discussion"
        : "Edit Discussion";
    }
  },
  watch: {
    dialog(val) {
      if (!val) val || this.close();
    },

    discussionDialog(val) {
      val || this.closeviewingDiscussion();
    },

    pagination: {
      handler() {
        this.getPosts();
      },
      deep: true
    },

    search: {
      handler(input) {
        this.searchPosts(input);
      }
    }
  },
  created() {
    //Get the list of courses first
    //For Dropdownlist
    this.$store
      .dispatch("GETALLCOURSES")
      .then(response => {
        this.courses = response.data;
      })
      .catch(err => {
        this.snackbar = true;
        this.color = "error";
        this.message = "Error, Please try again later";
      });

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
  methods: {
    stripHTML(item) {
      let text = striptags(item);
      return text;
    },
    getCourseName(courseId) {
      return this.courses.find(x => x.courseId === courseId).courseName;
    },
    getCourseFolderName(courseFolderId) {
      return this.courseFolders.find(x => x.courseFolderId === courseFolderId)
        .name;
    },
    getPosts() {
      this.loading = true;
      //Pass Search value into pagination object
      this.pagination.search = this.search;
      this.$store
        .dispatch("GETALLPOST", this.pagination)
        .then(response => {
          this.loading = false;
          this.DiscussionList = response.data;
          //Make the datetime more readable
          for (var i = 0; i < this.DiscussionList.length; i++) {
            this.DiscussionList[i].createdOn = moment.utc(
              this.DiscussionList[i].createdOn
            ).format("DD/MM/YY HH:mm:ss");
            if (
              this.DiscussionList[i].hasOwnProperty("courseId") &&
              this.DiscussionList[i].courseId != null
            ) {
              this.DiscussionList[i].courseName = this.getCourseName(
                this.DiscussionList[i].courseId
              );
            }
            if (this.DiscussionList[i].courseFolderId.length != 0) {
              this.DiscussionList[
                i
              ].courseFolderName = this.getCourseFolderName(
                this.DiscussionList[i].courseFolderId[0]
              );
            }
          }
          var xpg = JSON.parse(response.headers["x-pagination"]);
          this.pagination.totalItems = xpg["totalCount"];
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
    },

    searchPosts(input) {
      if (input.length > 3 || input.length == 0) {
        //API Call
        this.getPosts();
      }
    },

    viewDiscussion(discussion) {
      this.editedIndex = -2;
      this.viewingDiscussion = Object.assign({}, discussion);
      if (
        this.viewingDiscussion.hasOwnProperty("courseId") &&
        this.viewingDiscussion.courseId != null
      ) {
        var courseName = this.getCourseName(this.viewingDiscussion.courseId);
        this.viewingDiscussion.courseName = courseName;
      }
      if (
        this.viewingDiscussion.hasOwnProperty("courseFolderId") &&
        this.viewingDiscussion.courseFolderId != null
      ) {
        var courseFolderName = this.getCourseFolderName(
          this.viewingDiscussion.courseFolderId[0]
        );
        this.viewingDiscussion.courseFolderName = courseFolderName;
      }
      this.discussionDialog = true;
    },

    editDiscussion(discussion) {
      this.editedIndex = this.DiscussionList.indexOf(discussion);
      this.editedDiscussion = Object.assign({}, discussion);
      this.editedDiscussion.courseFolderId = this.editedDiscussion.courseFolderId[0];
      this.dialog = true;
    },

    close() {
      this.editedDiscussion = {};
      this.$refs.createform.reset();
      this.dialog = false;
      this.isViewing = false;

      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },

    closeviewingDiscussion() {
      this.editedIndex = -1;
      for (let keys in this.viewingDiscussion)
        delete this.viewingDiscussion[keys];
      this.discussionDialog = false;
    },

    save() {
      if (this.editedIndex > -1) {
        this.submitEditDiscussion();
      } else {
        this.createDiscussion();
      }
    },

    createDiscussion() {
      //Add all the required fields before passing it over
      this.editedDiscussion.createdBy = $cookies.get("userid");
      this.editedDiscussion.createdOn = moment(new Date());
      this.editedDiscussion.userName = $cookies.get("username");
      var id = this.editedDiscussion.courseFolderId;

      this.editedDiscussion.courseFolderId = [];
      this.editedDiscussion.courseFolderId.push(id);
      var item = this.editedDiscussion;
      delete item.courseName;
      delete item.courseFolderName;

      this.editedDiscussion = this.$store
        .dispatch("CREATEDISCUSSION", item)
        .then(response => {
          this.DiscussionList.push(this.editedDiscussion);
          this.pagination.totalItems = this.DiscussionList.length;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Create Error, Please try again later";
        });
      this.close();
    },

    submitEditDiscussion() {
      //Get the course folder id and name
      //Update on another variable then pass back to table
      var edited = this.editedDiscussion;
      var courseFolderId = this.editedDiscussion.courseFolderId;
      var courseFolderName = this.getCourseFolderName(courseFolderId);
      edited.courseFolderName = courseFolderName;
      edited.courseFolderId = [];
      edited.courseFolderId.push(courseFolderId);
      edited.createdOn = moment(this.editedDiscussion.createdOn).toDate();

      this.$store
        .dispatch("UPDATEDISCUSSION", edited)
        .then(response => {
          let discussionIndex = this.DiscussionList.findIndex(
            discussion => discussion.postId === edited.postId
          );
          edited.createdOn = moment(edited.createdOn).format(
            "DD/MM/YY HH:mm:ss"
          );
          this.DiscussionList.splice(discussionIndex, 1, edited);
          this.pagination.totalItems = this.DiscussionList.length;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Update Error, Please try again later";
        });
      this.close();
    },

    deleteDiscussion(discussion) {
      var answer = window.confirm(
        "Confirm delete Discussion Post? All replies made to the thread will be removed too"
      );
      if (answer) {
        this.$store
          .dispatch("DELETEDISCUSSION", discussion)
          .then(response => {
            this.loading = true;
            let discussionIndex = this.DiscussionList.findIndex(
              item => item.postId === discussion.postId
            );
            if (~discussionIndex) {
              this.DiscussionList.splice(discussionIndex, 1);
              this.loading = false;
            }
            this.pagination.totalItems = this.DiscussionList.length;
          })
          .catch(err => {
            this.snackbar = true;
            this.color = "error";
            this.message = "Error, Please try again later";
          });
      }
    }
  }
};
</script>
<style>
.viewEditor .ck-sticky-panel__content {
  display: none;
}

.ck-editor__editable {
  min-height: 200px;
}

.ck.ck-toolbar {
  display: block !important;
}
</style>
