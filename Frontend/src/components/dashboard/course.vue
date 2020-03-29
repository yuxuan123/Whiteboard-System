<template>
  <v-container fluid grid-list-xl fill-height>
    <v-layout justify-center>
      <v-flex xs12>
        <material-card color="general">
          <div slot="header">
            <div class="title font-weight-light mb-2">
              Courses
            </div>
          </div>
          <v-flex md12 sm12>
            <div>
              <v-snackbar v-model="snackbar" :color="color" :top="true">
                {{ messages }}
                <v-btn dark flat @click="snackbar = false">
                  Close
                </v-btn>
              </v-snackbar>
              <v-dialog v-model="dialog" width="600px">
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-if="userRole !== 'student'"
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
                    <v-spacer />
                    <v-btn class="general" rounded dark @click="addCourse">
                      Create
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-dialog>

              <v-dialog v-model="dialogleafNode" width="800px">
                <v-card>
                  <v-card-title class="headline">
                    Create document
                  </v-card-title>

                  <v-card-text>
                    <v-container class="pt-0" grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm12 md12>
                          <v-text-field
                            v-model="createMaterial.title"
                            label="Title"
                          />
                        </v-flex>
                        <v-flex xs12 sm12 md12>
                          <v-text-field
                            v-model="createMaterial.type"
                            label="Type: | announcement | lecture_video | lecture_slides |"
                          />
                        </v-flex>
                        <v-flex xs12 sm12 md12>
                          <v-text-field
                            v-model="createMaterial.Url"
                            label="URL"
                          />
                        </v-flex>

                        <v-flex xs12 sm12 md12>
                          <v-text-field
                            v-model="createMaterial.description"
                            label="Description"
                          />
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>

                  <v-card-actions>
                    <v-spacer />

                    <v-btn class="general" rounded dark @click="addleafNode">
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
                @change-name="onChangeName"
                @delete-node="deleteCourseCodeNode"
                @add-node="onAddNode"
              >
                <span slot="leafNodeIcon" class="icon">📃</span>
                <span slot="treeNodeIcon" class="icon">📂</span>
                <span slot class="icon">🌲</span>
              </vue-tree-list>
            </div>
          </v-flex>
        </material-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import axios from "axios";
import $ from "jquery";
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
      createMaterial: {},
      userId: "",
      userRole: "",
      dialog: false,
      dialogleafNode: false,
      nodeID: "",
      snackbar: false,
      color: "general",
      messages: ""
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
  mounted: function() {
    let cur = this;
    cur.$nextTick(function() {
      setTimeout(function() {
        // Code that will run only after the
        // entire view has been rendered
        cur.addButton();
      }, 1000);
      //zconsole.log(this.courseContents.length);
    });
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
            dragDisabled: true,
            addTreeNodeDisabled: true,
            editNodeDisabled: true
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
              dragDisabled: true,
              addTreeNodeDisabled: true
            });
          }
          (nodeleaf.id = this.courseContents[i].contentId),
            (nodeleaf.name =
              this.courseContents[i].title +
              " | " +
              this.courseContents[i].description +
              " | " +
              this.courseContents[i].url +
              " | ");
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
    addButton() {
      var body = document.getElementsByClassName("vtl-operation");
      var contentBody = document.getElementsByClassName("vtl-node-content");
      var stringURL;
      var URL = [];
      var leafNodeLocation = [];
      let cur = this;
      for (var k = contentBody.length; k > 0; k--) {
        var innHTML = contentBody[k - 1].innerHTML.trim();

        var l = innHTML.length;

        if (innHTML.charAt(l - 1) == "|") {
          leafNodeLocation.push(k - 1);
          for (var j = l - 2; j > 0; j--) {
            if (innHTML.charAt(j) != "|") {
              if (j + 1 == l - 2) {
                stringURL = innHTML.charAt(j);
              } else {
                stringURL = innHTML.charAt(j) + stringURL;
              }
            } else {
              URL.push(stringURL.trim());
              break;
            }
          }
        }
      }

      for (var i = 0; i < leafNodeLocation.length; i++) {
        // 1. Create the button
        var button = document.createElement("button");
        button.setAttribute("id", "btn" + i);
        button.setAttribute("class", "general downloadBtn");
        button.innerHTML = "Download";
        button.style.color = "#ffffff";
        button.style.float = "right";

        // 2. Append somewhere
        $(body[leafNodeLocation[i]]).append(button);
      }
      // // 3. Add event handler
      $(document).ready(function() {
        for (var i = 0; i < leafNodeLocation.length; i++) {
          var btnID = "#btn" + i;
          var fileName;
          // URL =
          // "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

          var fileTypeLength = 0;
          var longStringURL = URL[i];
          for (var v = longStringURL.length; v > 0; v--) {
            if (longStringURL[v] != "/") {
              if (v + 1 == longStringURL.length) {
                fileName = longStringURL[v];
              } else {
                fileName = longStringURL[v] + fileName;
              }
            } else {
              break;
            }
          }
          // onclick to perform download
          $(btnID).click(function() {
            axios({
              url: longStringURL,
              method: "GET",
              responseType: "blob" // important
            })
              .then(response => {
                var type = response.data.type;
                const url = window.URL.createObjectURL(
                  new Blob([response.data])
                );
                const link = document.createElement("a");
                link.href = url;

                link.setAttribute("download", fileName);
                document.body.appendChild(link);
                link.click();
              })
              .catch(err => {
                cur.snackbar = true;
                cur.color = "error";
                cur.messages = "Sorry cannot download this content!";
                //reject(err);
              });
          });
        }
      });
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
      this.dialogleafNode = true;
      this.nodeID = params.id;
      //console.log(params.name);
    },
    addleafNode() {
      let cur = this;
      var dobreak = false;
      var orgData = cur.courseCodeTree;
      var dataID;
      var URL;

      var locationbtn = 0;
      var leafNodeLocation = 0;

      //loop through the tree to modify the leaf node content after the creation of content via the api
      // first layer
      for (var i = 0; !dobreak && i < orgData.children.length; i++) {
        // second layer
        for (
          var k = 0;
          orgData.children[i].children != null &&
          k < orgData.children[i].children.length;
          k++
        ) {
          if (cur.nodeID == orgData.children[i].children[k].id) {
            //attempting to create content
            var content = {
              contentId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              courseId: orgData.children[i].id,
              Type: cur.createMaterial.type,
              Title: cur.createMaterial.title,
              Description: cur.createMaterial.description,
              Datetime: "0001-01-01T00:00:00Z",
              FileName: "null",
              Url: cur.createMaterial.Url,
              CreatedOn: "0001-01-01T00:00:00Z",
              CreatedBy: cur.userId
            };
            this.$store.dispatch("CREATECONTENT", content).then(response => {
              //console.log(response.data.contentId);

              dataID = response.data.contentId;
              URL = response.data.url;
            });
            //edit the data from tree
            orgData.children[i].children[k].id = dataID;
            orgData.children[i].children[k].name =
              cur.createMaterial.title +
              " | " +
              cur.createMaterial.description +
              " | " +
              cur.createMaterial.Url +
              " | ";
            dobreak = true;
            break;
          }
          leafNodeLocation++;
          locationbtn++;
        }
        locationbtn++;
      }
      //adding button after the tree data is updated
      setTimeout(function() {
        // Code that will run only after the
        // entire view has been rendered
        // 1. Create the button
        var button = document.createElement("button");
        //button.setAttribute("id", orgData.children[i].id);
        button.setAttribute("class", "general");
        button.innerHTML = "Download";
        button.style.color = "#ffffff";
        button.style.float = "right";
        // 2. Append somewhere
        var body = document.getElementsByClassName("vtl-operation");
        $(body[locationbtn]).append(button);

        $(document).ready(function() {
          var btnID = "#" + orgData.children[i].id;
          var fileName;
          // var URL =cur.createMaterial.Url;
          //console.log(cur.createMaterial.Url);
          // "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

          for (var v = URL.length; v > 0; v--) {
            if (URL[v] != "/") {
              if (v + 1 == URL.length) {
                fileName = URL[v];
              } else {
                fileName = URL[v] + fileName;
              }
            } else {
              break;
            }
          }
          // onclick to perform download
          $(btnID).click(function() {
            axios({
              url: URL,
              method: "GET",
              responseType: "blob" // important
            })
              .then(response => {
                var type = response.data.type;
                const url = window.URL.createObjectURL(
                  new Blob([response.data])
                );
                const link = document.createElement("a");
                link.href = url;

                link.setAttribute("download", fileName);
                document.body.appendChild(link);
                link.click();
              })
              .catch(err => {
                cur.snackbar = true;
                cur.color = "error";
                cur.messages = "Sorry cannot download this content!";
                //  reject(err);
              });
          });
        });
        var Divbody = document.getElementsByClassName("vtl-node vtl-leaf-node");

        Divbody[leafNodeLocation].setAttribute("id", dataID);
      }, 1500);

      cur.createMaterial = {};
      cur.nodeID = "";
      cur.dialogleafNode = false;
    },

    addNode() {
      var node = new TreeNode({ name: "new node", isLeaf: false });
      if (!this.data.children) this.data.children = [];
      this.data.addChildren(node);
    },

    onChangeName(params) {
      var title = "1";
      var description = "1";
      var url = "1";
      var count = 1;

      for (var p = 0; p < params.newName.length; p++) {
        if (params.newName.charAt(p) != "|") {
          if (count == 1) {
            if (title == "1") {
              title = params.newName.charAt(p);
            } else {
              title = title + params.newName.charAt(p);
            }
          } else if (count == 2) {
            if (description == "1") {
              description = params.newName.charAt(p);
            } else {
              description = description + params.newName.charAt(p);
            }
          } else if (count == 3) {
            if (url == "1") {
              url = params.newName.charAt(p);
            } else {
              url = url + params.newName.charAt(p);
            }
          }
        } else if (params.newName.charAt(p) == "|") {
          count++;
        }
      }
      title = title.trim();
      description = description.trim();
      url = url.trim();

      //console.log(description);
      var myObj = {
        contentId: params.id,
        courseId: params.id,
        type: "null",
        title: title,
        description: description,
        datetime: "0001-01-01T00:00:00Z",
        fileName: url,
        url: url,
        createdOn: "0001-01-01T00:00:00Z",
        createdBy: this.userId //"d99dfc08-5d7a-4e04-c824-08d7c5959f4d"
      };
      this.axios.put(
        "https://whiteboardsyetem.azurewebsites.net/updateContent",
        myObj
      );
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
.downloadBtn {
  margin-left: 10px;
  line-height: 40px;
  font-weight: bold;
  padding: 0 20px;
  background: #337ab7;
  border: #2e6da4;
  border-radius: 25px;
}
.downloadBtn:hover {
  color: #fff;
  background: #337ab7;
  border-radius: 25px;
}
</style>
