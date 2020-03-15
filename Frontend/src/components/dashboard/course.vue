

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
                  <v-btn rounded dark class="general" id="btnA" v-if="!role==='student'" v-on="on">Add Courses</v-btn>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="headline">Create New Course Folder</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container class="pt-0" grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm6 md6>
                          <v-text-field v-model="createContent.CourseCode" label="Course Code" />
                        </v-flex>

                        <v-flex xs12 sm6 md6>
                          <v-text-field v-model="createContent.CourseName" label="Course Name" />
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

                    <v-btn color="green darken-1" flat="flat" @click="addCourse">Create</v-btn>
                  </v-card-actions>
                </v-card>
              </v-dialog>

              <vue-tree-list
                @click="onClick"
                @change-name="onChangeName"
                @delete-node="onDel"
                @add-node="onAddNode"
                :model="CourseCodeNo"
                default-tree-node-name="new Course"
                default-leaf-node-name="new Material"
                v-bind:default-expanded="false"
              >
                <span class="icon" slot="leafNodeIcon">📃</span>
                <span class="icon" slot="treeNodeIcon">📂</span>
                <span class="icon" slot>🌲</span>
              </vue-tree-list>

              <!-- <button @click="fetchData">Get new tree</button>
              <span class="icon" slot="treeNodeIcon">📂</span>
               <span class="icon" slot="addLeafNodeIcon">＋</span>
                <span class="icon" slot="editNodeIcon">📃</span>
              <span class="icon" slot="delNodeIcon">✂️</span>
              <span class="icon" slot="addTreeNodeIcon">📂</span>-->
            </div>
          </v-flex>
        </material-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>
<script src="https://unpkg.com/axios/dist/axios.min.js"></script>
<script>
import { VueTreeList, Tree, TreeNode } from "vue-tree-list";
export default {
  components: {
    VueTreeList
  },
  data() {
    return {
      newTree: {},
      CourseCodeNo: new Tree([]),
      createContent: {},
      dialog: false
    };
  },

  created() {
    var orgData = this.CourseCodeNo;
    let userId = $cookies.get("userid");

    var arrayCoruseName = [];
    var arrayCourseCode = [];
    var arrayCodeID = [];
    
    var role;
    const axios = require("axios");
    async function userCheck() {
      await axios
        .get(
          "https://whiteboardsyetem.azurewebsites.net/getUsers?userIds=" +
            userId
        )
        .then(function(response) {
          role = response.data.userDtos[0].role;
        });
      insertDataIntoArray();
    }

    async function insertDataIntoArray() {
      await axios
        .get("https://whiteboardsyetem.azurewebsites.net/getAllCourses")
        .then(function(response) {
          for (var i = 0; i < response.data.length; i++) {
            arrayCourseCode.push(response.data[i].courseCode);
            arrayCodeID.push(response.data[i].courseId);
            arrayCoruseName.push(response.data[i].courseName);
          }
          insertDataIntoTree();
        });
    }

    async function insertDataIntoTree() {
      axios
        .get("https://whiteboardsyetem.azurewebsites.net/getContent/" + userId)
        .then(function(response) {
          for (var k = 0; k < arrayCourseCode.length; k++) {
            var node;
            if (role === "student") {
              node = new TreeNode({
                id: arrayCodeID[k],
                name: arrayCourseCode[k] + " " + arrayCoruseName[k],
                isLeaf: false,
                dragDisabled: true,
                addTreeNodeDisabled: true,
                addLeafNodeDisabled: true,
                editNodeDisabled: true,
                delNodeDisabled: true
              });
            } else {
              node = new TreeNode({
                id: arrayCodeID[k],
                name: arrayCourseCode[k] + " " + arrayCoruseName[k],
                isLeaf: false,
                dragDisabled: true
              });
            }

            if (!orgData.children) {
              orgData.children = [];
            }

            orgData.addChildren(node);

            for (var l = 0; l < response.data.length; l++) {
              if (
                response.data[l].courseId === arrayCodeID[k] &&
                (response.data[l].type === "lecture_slides" ||
                  response.data[l].type === "lecture_video")
              ) {
                var nodeleaf;
                if (role === "student") {
                  nodeleaf = new TreeNode({
                    id: response.data[l].contentId,
                    name:
                      response.data[l].title +
                      "/       Url:" +
                      response.data[l].url,
                    isLeaf: true,
                    dragDisabled: true,
                    addTreeNodeDisabled: true,
                    addLeafNodeDisabled: true,
                    editNodeDisabled: true,
                    delNodeDisabled: true
                  });
                } else {
                  nodeleaf = new TreeNode({
                    id: response.data[l].contentId,
                    name:
                      response.data[l].title +
                      "/       Url:" +
                      response.data[l].url,
                    isLeaf: true,
                    dragDisabled: true
                  });
                }
                orgData.children[k].addChildren(nodeleaf);
              }
            }
          }
        });
    }
    userCheck();
  },
  methods: {
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
      console.log(node);
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
      let cur=this;
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
            if (!cur.CourseCodeNo.children) cur.CourseCodeNo.children = [];
            cur.CourseCodeNo.addChildren(node);
            cur.dialog=false;
            cur.createContent={};
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
            // Save it!
          } else {
            // Do nothing!
          }
        });
    }
  }
};
</script>

// <style lang="less" rel="stylesheet/less">
// .vtl {
//   .vtl-drag-disabled {
//     background-color: #d0cfcf;
//     &:hover {
//       background-color: #d0cfcf;
//     }
//   }
//   // .vtl-disabled {
//   //   background-color: #d0cfcf;
//   // }
// }
//
</style>

<style lang="less" rel="stylesheet/less" scoped>
.icon {
  &:hover {
    cursor: pointer;
  }
}
</style>
