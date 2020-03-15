<template>
  <v-container fluid grid-list-xl fill-height>
    <v-layout justify-center>
      <v-flex xs12>
        <material-card color="general">
          <div slot="header">
            <div class="title font-weight-light mb-2">Courses</div>
          </div>
          <v-flex md12 sm12>
            <div>
              <v-dialog v-model="dialog" width="600px">
                <template v-slot:activator="{ on }">
                  <v-btn
                    rounded
                    dark
                    class="general"
                    v-if="!userRole === 'student'"
                    v-on="on"
                    >Add Course</v-btn
                  >
                </template>
                <v-card>
                  <v-card-title>
                    <span class="headline">Create Course</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container class="pt-0" grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="createContent.CourseCode"
                            label="Course Code"
                          />
                        </v-flex>

                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="createContent.CourseName"
                            label="Course Name"
                          />
                        </v-flex>

                        <v-flex xs12 sm12 md12>
                          <v-text-field
                            v-model="createContent.CourseDescription"
                            label="Course Description"
                          />
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="green darken-1" flat="flat" @click="addCourse"
                      >Create</v-btn
                    >
                  </v-card-actions>
                </v-card>
              </v-dialog>

              <vue-tree-list
                @click="onClick"
                @change-name="onChangeName"
                @delete-node="onDel"
                @add-node="onAddNode"
                :model="courseCodeTree"
                default-tree-node-name="new Course"
                default-leaf-node-name="new Content"
                v-bind:default-expanded="false"
              >
                <span class="icon" slot="leafNodeIcon">📃</span>
                <span class="icon" slot="treeNodeIcon">📂</span>
                <span class="icon" slot>🌲</span>
              </vue-tree-list>
            </div>
          </v-flex>
        </material-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script>
import { VueTreeList, Tree, TreeNode } from "vue-tree-list";
export default {
  components: {
    VueTreeList
  },
  data() {
    return {
      courseCodeTree: new Tree([]),
      courses: [],
      courseContents: [],
      createContent: {},
      userId: "",
      userRole: "",
      dialog: false
    };
  },
  created() {
    //Pass user data from cookies
    this.userId = $cookies.get("userid");
    this.userRole = $cookies.get("role");

    //Check if the user role is a student or professor
    //Get User courses or All Course
    //Add the course to tree
    if (this.userRole === "student") {
      this.getUserCourses();
    } else {
      this.getAllCourses();
    }
  },
  methods: {
    getUserCourses() {
      this.$store.dispatch("GETUSERCOURSES", this.userId).then(response => {
        this.courses = response.data;
        this.getCourseContent(this.userId);
      });
    },
    getAllCourses() {
      this.$store.dispatch("GETALLCOURSES").then(response => {
        this.courses = response.data;
        this.getCourseContent(this.userId);
      });
    },
    getCourseContent(userId) {
      this.$store.dispatch("GETUSERCONTENT", userId).then(response => {
        this.courseContents = response.data;
        this.addTreeNodeChildren();
      });
    },
    addTreeNodeChildren() {
      var node = {};
      for (var i = 0; i < this.courses.length; i++) {
        if (this.userRole === "student") {
          node = new TreeNode({
            //Set Default config for student
            isLeaf: false,
            dragDisabled: true,
            addTreeNodeDisabled: true,
            addLeafNodeDisabled: true,
            editNodeDisabled: true,
            delNodeDisabled: true
          });
        } else {
          node = new TreeNode({
            //Set Default config for student
            isLeaf: false,
            dragDisabled: true
          });
        }
        node.id = this.courses[i].courseId;
        node.name =
          this.courses[i].courseCode + " - " + this.courses[i].courseName;
        this.courseCodeTree.addChildren(node);
        this.addTreeNodeLeaf(this.courses[i].courseId, i);
      }
    },
    addTreeNodeLeaf(courseId, index) {
      for (var i = 0; i < this.courseContents.length; i++) {
        if (
          this.courseContents[i].courseId === courseId &&
          this.courseContents[i].type !== "announcement"
        ) {
          var nodeleaf;
          if (this.userRole === "student") {
            nodeleaf = new TreeNode({
              isLeaf: true,
              dragDisabled: true,
              addTreeNodeDisabled: true,
              addLeafNodeDisabled: true,
              editNodeDisabled: true,
              delNodeDisabled: true
            });
          } else {
            nodeleaf = new TreeNode({
              isLeaf: true,
              dragDisabled: true
            });
          }
          (nodeleaf.id = this.courseContents[i].contentId),
            (nodeleaf.name =
              this.courseContents[i].title +
              " | " +
              this.courseContents[i].description +
              " | " +
              this.courseContents[i].url);
          this.courseCodeTree.children[index].addChildren(nodeleaf);
        }
      }
    },
    onDel(node) {
      if (
        (node.name.charAt(0) === "C" || node.name.charAt(0) === "c") &&
        (node.name.charAt(1) === "Z" || node.name.charAt(1) === "z")
      ) {
        this.axios
          .delete(
            "https://whiteboardsyetem.azurewebsites.net/deleteCourse/" + node.id
          )
          .then(function(response) {});
      } else {
        this.axios
          .delete(
            "https://whiteboardsyetem.azurewebsites.net/deleteContent/" +
              node.id
          )
          .then(function(response) {});
      }
      node.remove();
    },

    onChangeName(params) {
      var myObj = {
        contentId: params.id,
        courseId: params.id,
        type: "null",
        title: params.newName,
        description: "null",
        datetime: "0001-01-01T00:00:00Z",
        fileName: params.newName + ".doc",
        url: params.newName + ".doc",
        createdOn: "0001-01-01T00:00:00Z",
        createdBy: "d99dfc08-5d7a-4e04-c824-08d7c5959f4d"
      };
      this.axios
        .put("https://whiteboardsyetem.azurewebsites.net/updateContent", myObj)
        .then(function(response) {});
      console.log(params);
    },

    onAddNode(params) {
      console.log(params.name);
    },

    addNode() {
      var node = new TreeNode({ name: "new node", isLeaf: false });
      if (!this.data.children) this.data.children = [];
      this.data.addChildren(node);
    },
    addCourse() {
      let cur = this;
      let role = $cookies.get("role");
      let userId = $cookies.get("userid");
      console.log(this.createContent);
      this.createContent.CreatedBy = "70e2777c-401a-4769-4721-08d7c2644faa";

      if (role === "student") {
        this.axios
          .post(
            "https://whiteboardsyetem.azurewebsites.net/createCourse?" +
              "CourseCode=" +
              this.createContent.CourseCode +
              "&CourseName=" +
              this.createContent.CourseName +
              "&CourseDescription=" +
              this.createContent.CourseDescription +
              "&CreatedBy=" +
              this.createContent.CreatedBy
          )
          .then(function(response) {
            console.log(response);
            var node = new TreeNode({
              name: response.data.courseName,
              isLeaf: false,
              dragDisabled: true,
              addLeafNodeDisabled: true,
              editNodeDisabled: true
            });
            if (!cur.courseCodeTree.children) cur.courseCodeTree.children = [];
            cur.courseCodeTree.addChildren(node);
            cur.dialog = false;
            cur.createContent = {};
          });
      }
    },
    onClick(params) {
      let userId = $cookies.get("userid");
      var role;

      this.axios
        .get(
          "https://whiteboardsyetem.azurewebsites.net/getUsers?userIds=" +
            userId
        )
        .then(function(response) {
          role = response.data.userDtos[0].role;
          if (
            params.name.charAt(0) !== "C" &&
            params.name.charAt(0) !== "c" &&
            params.name.charAt(0) !== "Z" &&
            params.name.charAt() !== "z" &&
            role === "student"
          ) {
            confirm("Do you want to download this?");
          }
        });
    }
  }
};
</script>
