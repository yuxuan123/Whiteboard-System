

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
              <v-btn
                rounded
                dark
                class="general"
                id="btnA"
                @click="addCourse"
                v-if="!role==='student'"
              >Add Courses</v-btn>
              <vue-tree-list
                @click="onCslick"
                @change-name="onChangeName"
                @delete-node="onDel"
                @add-node="onAddNode"
                :model="CourseCodeNo"
                default-tree-node-name="new Course"
                default-leaf-node-name="new Material"
                v-bind:default-expanded="false"
                v-bind:showCheckBox="true"
              >
                <span class="icon" slot="addLeafNodeIcon">＋</span>
                <span class="icon" slot="editNodeIcon">📃</span>
                <span class="icon" slot="delNodeIcon">✂️</span>
                <span class="icon" slot="leafNodeIcon">📃</span>
                <span class="icon" slot="treeNodeIcon">📂</span>
                <span class="icon">🌲</span>
                
              </vue-tree-list>
              
              <!-- <button @click="fetchData">Get new tree</button>
              <span class="icon" slot="treeNodeIcon">📂</span>
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
      CourseCodeNo: new Tree([])
    };
  },

  created() {
    var orgData = this.CourseCodeNo;
    let userId = $cookies.get("userid");

    var arrayCoruseName = [];
    var arrayCourseCode = [];
    var arrayCodeID = [];
    const axios = require("axios");
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
            var node = new TreeNode({
              name: arrayCourseCode[k] + " " + arrayCoruseName[k],
              isLeaf: false,
              dragDisabled: true,
              addTreeNodeDisabled: true,
              addLeafNodeDisabled: true,
              editNodeDisabled: true,
              delNodeDisabled: true
            });
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
                var nodeleaf = new TreeNode({
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

                orgData.children[k].addChildren(nodeleaf);
              }
            }
          }
        });
    }
    insertDataIntoArray();
  },
  methods: {
    onDel(node) {
      console.log(node);
      node.remove();
    },

    onChangeName(params) {
      console.log(params);
    },

    onAddNode(params) {
      console.log(params);
    },

    onClick(params) {
      console.log(params);
    },

    addNode() {
      var node = new TreeNode({ name: "new node", isLeaf: false });
      if (!this.data.children) this.data.children = [];
      this.data.addChildren(node);
    },
    addCourse() {
      let userId = $cookies.get("userid");
      var role;
      this.axios
        .get(
          "https://whiteboardsyetem.azurewebsites.net/getUsers?userIds=" +
            userId
        )
        .then(function(response) {
          role = response.data.userDtos[0].role;
        });

      if (!role === "student") {
        var node = new TreeNode({ name: "new Course", isLeaf: false });
        if (!this.CourseCodeNo.children) this.CourseCodeNo.children = [];
        this.CourseCodeNo.addChildren(node);
      } else {
        alert("Only Prof are allow to add new course.");
      }
    },
    onCslick() {
      if (confirm("Do you want to download this?")) {
        // Save it!
      } else {
        // Do nothing!
      }
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
