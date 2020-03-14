<!--<template>
  <v-layout
  column>
    <h1>Pusher Test</h1>
    <p>
      Publish an event to channel <code>my-channel</code> with event name
      <code>my-event</code>; it will appear below:
    </p>
    <v-flex>
      <ul>
        <li v-for="message in messages" :key="message.id">
          {{ message }}
        </li>
      </ul>
      <v-layout justify-center>
      <v-flex xs12>
        <material-card color="general">
          <div slot="header">
            <div class="title font-weight-light mb-2">
              Courses
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
                  Course Code
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
    </v-flex>
    
  </v-layout>
</template>

<script>
import Pusher from "pusher-js";

Pusher.logToConsole = true;

var pusher = new Pusher("0a3b3bc361a655ea56ac", {
  cluster: "ap1",
  forceTLS: true
});

export default {
  data: () =>
   ({
    messages: [],
     notifications: [{ id: "1", info: "ac" }, { id:"2", info: "abc" }, { id: "3", info: "acbc" }]
  
  }),
   
  created() {
    this.subscribe();
    this.sendMessage();
  },
 
   
  methods: {
    subscribe() {
      var pusher = new Pusher("0a3b3bc361a655ea56ac", {
        cluster: "ap1",
        forceTLS: true
      });
      pusher.subscribe("my-channel");
      pusher.bind("my-event", data => {
        this.messages.push(JSON.stringify(data));
      });
    },
    sendMessage() {
      this.axios({
        method: "post",
        url: "https://whiteboardsyetem.azurewebsites.net/pusher",
        data: {
          name: "Alan",
          message: "Test"
        },
        config: { headers: { "Content-Type": "application/json" } }
      }).then(response => {
        console.log("sent");
      });
    }
  }
};
</script>
-->

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
        <div class="title font-weight-light mb-2">Courses</div>
      </div>
      <v-flex md12 sm12>
        <div>
          <v-btn rounded 
           dark
            class="general"
            @click="addCourse">
           
            Add Courses</v-btn>
          <vue-tree-list
            @click="onCslick"
            @change-name="onChangeName"
            @delete-node="onDel"
            @add-node="onAddNode"
            :model="CourseCodeNo"
            default-tree-node-name="new Course"
            default-leaf-node-name="new Material"
            v-bind:default-expanded="false"
          >
            <span class="icon" slot="addTreeNodeIcon">📂</span>
            <span class="icon" slot="addLeafNodeIcon">＋</span>
            <span class="icon" slot="editNodeIcon">📃</span>
            <span class="icon" slot="delNodeIcon">✂️</span>
            <span class="icon" slot="leafNodeIcon">🍃</span>
            <span class="icon" slot="treeNodeIcon">🌲</span>
          </vue-tree-list>
          <!-- <button @click="fetchData">Get new tree</button> -->
          <!-- <pre>
      {{newTree}}
    </pre> -->
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
      CourseCodeNo: new Tree([]// [
      //   {
          
      //     name: "CZ0001",
      //     id: 1,
      //     pid: 0,
      //     // dragDisabled: true,
      //     addTreeNodeDisabled: true,
      //     addLeafNodeDisabled: true,
      //     //editNodeDisabled: true,
      //     //delNodeDisabled: true,
      //     children: [
      //       {
      //         name: "Node 1-2",
      //         id: 2,
      //         isLeaf: true,
      //         pid: 1
      //       }
      //     ]
      //   },
      //   {
      //     name: "CZ0002",
      //     id: 3,
      //     pid: 0
      //     //  disabled: true
      //   },
      //   {
      //     name: "CZ0003",
      //     id: 4,
      //     pid: 0
      //   }
      // ])
      )

      
    };
  },
 
  created() {
    
     var orgData=this.CourseCodeNo;
   
    this.axios
        .get("https://whiteboardsyetem.azurewebsites.net/getAllCourses")
        .then(function(response) {
        
          for (var i = 0; i < response.data.length; i++){
            
           
            var p=response.data[i].courseCode
          
             var node = new TreeNode({ name: p, isLeaf: false });
             if (!orgData.children) 
             {orgData.children = [];
               
             }
             orgData.addChildren(node);
              if(response.data[i].courseFolders!='')
             {
              var node1 = new TreeNode({ name: response.data[i].courseFolders[0].name, isLeaf: false });
                if (!orgData.children.children) 
             {orgData.children.children = [];
              
             }
             orgData.addChildren(node1);
              // alert(response.data[i].courseFolders);
               alert( response.data[i].courseFolders[0].name);
             }
          }
  
  });},
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
     
      var node = new TreeNode({ name: "new Course", isLeaf: false });
      if (!this.CourseCodeNo.children) this.CourseCodeNo.children = [];
       this.CourseCodeNo.addChildren(node);
    },

    getNewTree() {
      // var vm = this
      // function _dfs (oldNode) {
      //   var newNode = {}
      //   for (var k in oldNode) {
      //     if (k !== 'children' && k !== 'parent') {
      //       newNode[k] = oldNode[k]
      //     }
      //   }
      //   if (oldNode.children && oldNode.children.length > 0) {
      //     newNode.children = []
      //     for (var i = 0, len = oldNode.children.length; i < len; i++) {
      //       newNode.children.push(_dfs(oldNode.children[i]))
      //     }
      //   }
      //   return newNode
      // }
      // vm.newTree = _dfs(vm.data)
    }


  

  }
   
};
</script>

<style lang="less" rel="stylesheet/less">
.vtl {
  .vtl-drag-disabled {
    background-color: #d0cfcf;
    &:hover {
      background-color: #d0cfcf;
    }
  }
  .vtl-disabled {
    background-color: #d0cfcf;
  }
}
</style>

<style lang="less" rel="stylesheet/less" scoped>
.icon {
  &:hover {
    cursor: pointer;
  }
}
</style>
