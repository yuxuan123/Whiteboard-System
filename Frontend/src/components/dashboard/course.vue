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
              Courses
            </div>
          </div>
          <v-flex
            md12
            sm12
          >
            <div>
              <v-dialog
                v-model="dialog"
                width="600px"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-if="$cookies.get('userid') != 'student'"
                    rounded
                    dark
                    class="general"
                    v-on="on"
                  >
                    Add Course
                  </v-btn>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="headline">Create Course</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container
                      class="pt-0"
                      grid-list-md
                    >
                      <v-layout wrap>
                        <v-flex
                          xs12
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="createContent.CourseCode"
                            label="Course Code"
                          />
                        </v-flex>

                        <v-flex
                          xs12
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="createContent.CourseName"
                            label="Course Name"
                          />
                        </v-flex>

                        <v-flex
                          xs12
                          sm12
                          md12
                        >
                          <v-text-field
                            v-model="createContent.CourseDescription"
                            label="Course Description"
                          />
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                  <v-card-actions>
                    <v-spacer />
                    <v-btn
                      color="green darken-1"
                      flat="flat"
                      @click="addCourse"
                    >
                      Create
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-dialog>

              <vue-tree-list
                class="mt-3"
                :model="courseCodeTree"
                default-tree-node-name="new Course"
                default-leaf-node-name="new Content"
                :default-expanded="false"
                @click="onClick"
                @change-name="onChangeName"
                @delete-node="deleteCourseCodeNode"
                @add-node="onAddNode"
              >
                <span
                  slot="leafNodeIcon"
                  class="icon"
                >📃</span>
                <span
                  slot="treeNodeIcon"
                  class="icon"
                >📂</span>
                <span
                  slot
                  class="icon"
                >🌲</span>
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
    this.getUserCourses();
  },
  methods: {
    getUserCourses() {
      this.$store.dispatch("GETUSERCOURSES", this.userId).then(response => {
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

    addCourse() {
      let cur = this;
      this.createContent.CreatedBy = this.userId;
      if (this.userRole === "lecturer") {
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
            var node = new TreeNode({
              id: response.data.courseId,
              name: response.data.courseCode + " - " + response.data.courseName,
              isLeaf: false,
              dragDisabled: true,
              addLeafNodeDisabled: true,
              editNodeDisabled: true
            });
            if (!cur.courseCodeTree.children) cur.courseCodeTree.children = [];
            cur.courseCodeTree.addChildren(node);
            cur.dialog = false;
            cur.createContent = {};
            cur.regLecturerToCourse(
              response.data.courseId,
              response.data.createdBy
            );
          });
      }
    },

    regLecturerToCourse(courseId, createdBy) {
      var item = {
        courseId: courseId,
        staffId: createdBy,
        isActive: true
      };
      this.$store.dispatch("ADDCOURSESTAFF", item);
    },

    deleteCourseCodeNode(node) {
      var answer = window.confirm("Confirm delete?");
      if (answer) {
        if (
          (node.name.charAt(0) === "C" || node.name.charAt(0) === "c") &&
          (node.name.charAt(1) === "Z" || node.name.charAt(1) === "z")
        ) {
          this.$store.dispatch("DELETECOURSE", node.id);
        } else {
          this.$store.dispatch("DELETECONTENT", node.id);
        }
        node.remove();
      }
    },

    onAddNode(params) {
      console.log(params.name);
    },

    addNode() {
      var node = new TreeNode({ name: "new node", isLeaf: false });
      if (!this.data.children) this.data.children = [];
      this.data.addChildren(node);
    },

    onChangeName(params) {
      console.log(params);
    },

    onClick(params) {
      if (
        params.name.charAt(0) !== "C" &&
        params.name.charAt(0) !== "c" &&
        params.name.charAt(0) !== "Z" &&
        params.name.charAt() !== "z" &&
        this.userRole === "student"
      ) {
        confirm("Do you want to download this?");
      }
    }
  }
};
</script>
<style>
.vtl-node {
  padding: 1px !important;
  border-radius: 10px !important;
  border-color: #448aff;
  border-style: solid;
  border-width: 0.5px;
  margin-top: 10px !important;
  margin-bottom: 20px !important;
}

.vtl-node-main:hover {
  border-radius: 10px !important;
}
.vtl-node-content {
  padding: 10px !important;
  font-size: 18px !important;
}
</style>
