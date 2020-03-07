<template>
  <v-container
    fill-height
    fluid
    grid-list-xl
  >
    <v-layout
      justify-center
      wrap
    >
      <v-flex md12>
        <div>
          <material-card
            color="general"
            title="Manage Users"
            text="Manage user accounts"
          >
            <v-spacer />
            <br>
            <v-dialog
              v-model="userDialog"
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
                        sm6
                        md6
                      >
                        <v-text-field
                          v-model="viewingUser.name"
                          label="Name"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm12
                        md12
                      >
                        <v-text-field
                          v-model="viewingUser.email"
                          label="Email"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm12
                        md12
                      >
                        <v-text-field
                          v-model="viewingUser.phoneno"
                          label="Email"
                          readonly
                        />
                      </v-flex>

                      <v-flex
                        xs12
                        sm12
                        md12
                      >
                        <v-text-field
                          v-model="viewingUser.role"
                          label="Role"
                          readonly
                        />
                      </v-flex>
                    </v-layout>
                  </v-container>
                  <v-card-actions>
                    <v-spacer />
                    <v-btn
                      color="blue darken-1"
                      flat
                      @click="closeViewingUser"
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
                  class="mb-3"
                  v-on="on"
                >
                  New User
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
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="editedUser.name"
                            label="Name"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex
                          xs12
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="editedUser.email"
                            label="Email"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex
                          xs12
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="editedUser.phoneno"
                            label="Phone No"
                            :readonly="isViewing"
                          />
                        </v-flex>

                        <v-flex
                          xs6
                          sm6
                          md6
                        >
                          <v-text-field
                            v-model="editedUser.role"
                            label="Role"
                            :readonly="isViewing"
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
                    v-if="!this.isViewing"
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
              :items="UserList"
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
                <td>{{ props.item.username }}</td>
                <td>{{ props.item.email }}</td>
                <td>{{ props.item.phoneno }}</td>
                <td>{{ props.item.role }}</td>
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
                        @click="viewUser(props.item)"
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
                        @click="editUser(props.item)"
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
                        @click="emailUser(props.item)"
                      >
                        <v-icon
                          size="20"
                          color="blue lighten-1"
                        >
                          mdi-email
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="deleteUser(props.item)"
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
  </v-container>
</template>

<script>
export default {
  data: () => ({
    loading: false,
    isViewing: false,
    userDialog: false,
    search: "",
    pagination: {
      page: 1,
      rowsPerPage: 5,
      sortBy: "name",
      descending: true,
      rowsPerPageItems: [5, 10, 15],
      totalItems: 0
    },
    UserList: [
      {
        id: "1",
        username: "Alan",
        phoneno: "98765432",
        role: "User"
      },
      {
        id: "2",
        username: "Bill",
        phoneno: "91234567",
        role: "User"
      },
      {
        id: "3",
        username: "Charlie",
        phoneno: "92726262",
        role: "Admin"
      },
      {
        id: "4",
        username: "Delta",
        phoneno: "98272722",
        role: "User"
      },
      {
        id: "5",
        username: "Ethan",
        phoneno: "92726223",
        role: "Admin"
      }
    ],
    dialog: false,
    search: "",
    headers: [
      {
        text: "Username",
        value: "username"
      },
      { text: "Email", value: "email" },
      {
        text: "Phone Number",
        value: "phoneno",
        filterable: false,
        sortable: false
      },
      {
        text: "Role",
        value: "role"
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
    editedUser: {},
    defaultItem: {},
    viewingUser: {}
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "New User"
        : this.editedIndex === -2
        ? "View User"
        : "Edit User";
    }
  },
  watch: {
    dialog(val) {
      if (!val) val || this.close();
    },

    userDialog(val) {
      val || this.closeViewingUser();
    },

    pagination: {
      handler() {
        //this.getUsers();
      },
      deep: true
    },

    search: {
      handler(input) {
        //this.searchUser()
      }
    }
  },

  methods: {
    getUsers() {
      this.loading = true;
      const { page, rowsPerPage, sortBy, descending } = this.pagination;
      this.pagination.totalItems = this.UserList.length;
    },

    editUser(user) {
      this.editedIndex = this.UserList.indexOf(user);
      this.editedUser = Object.assign({}, user);
      this.dialog = true;
    },

    viewUser(user) {
      this.editedIndex = -2;
      this.viewingUser = Object.assign({}, user);
      this.userDialog = true;
    },
    emailUser(user) {
      var answer = window.confirm("Send activation email?");
      if (answer) {
        //Send Activation Email
      }
    },
    deleteUser(user) {
      var answer = window.confirm("Confirm delete user?");
      if (answer) {
        this.loading = true;
        let userIndex = this.UserList.findIndex(item => item.id === user.id);
        if (~userIndex) {
          this.UserList.splice(userIndex, 1);
          this.loading = false;
        }
        this.pagination.totalItems = this.pagination.totalItems - 1;
      }
    },

    close() {
      for (let keys in this.editedUser) delete this.editedUser[keys];

      this.$refs.createform.reset();

      this.dialog = false;
      this.isViewing = false;

      setTimeout(() => {
        this.editedIndex = -1;
      }, 300);
    },

    closeViewingUser() {
      this.editedIndex = -1;
      for (let keys in this.viewingUser) delete this.viewingUser[keys];
      this.userDialog = false;
    },

    save() {
      if (this.editedIndex > -1) {
        this.submitEditUser();
      } else {
        this.createUser();
      }
    },

    createUser() {
      this.pagination.totalItems = this.pagination.totalItems + 1;
      this.UserList.push(this.editedUser);
      this.close();
    },

    submitEditUser() {

 let userIndex = this.UserList.findIndex(
            user => user.id === this.editedUser.id
          );

          this.UserList.splice(userIndex, 1, this.editedUser)
          this.close();

          this.loading = true;
    },

    searchUser(input) {
      if (input.length > 2) {
          //API Call
      }
    },

    


    cropSuccess(imgDataUrl, field) {
      this.imgDataUrl = imgDataUrl;
    },
    cropUploadSuccess(res) {
      if (res.type == "cover") {
        this.coverImgDir.push(res.fileUrl);
      } else if (res.type == "profile") {
        this.profileImgDir.push(res.fileUrl);
      }
    },
    cropUploadFail(status, field) {
      console.log("imageUploadFailed");
    },

    toggleShow(name) {
      if (name == "cover") {
        this.showCoverUpload = !this.showCoverUpload;
      } else if (name == "profile") {
        this.showProfileUpload = !this.showProfileUpload;
      }
    },

    removeUnusedImages() {
      let junkImageList = this.coverImgDir.concat(this.profileImgDir);
      if (junkImageList.length <= 0) return;

      let deleteImageList = {
        fileUrl: junkImageList
      };

      this.axios({
        method: "delete",
        url: "/api/uploads/clear",
        data: deleteImageList,
        config: { headers: { "Content-Type": "application/json" } }
      });

      this.posterImgDir.splice(0, this.posterImgDir.length);
      this.approvalImgDir.splice(0, this.approvalImgDir.length);
    },

    getCoverProfileUrl() {
      if (this.profileImgDir.length > 0) {
        this.editedUser.profileUrl = this.profileImgDir[
          this.profileImgDir.length - 1
        ];
      }

      if (this.coverImgDir.length > 0) {
        this.editedUser.coverUrl = this.coverImgDir[
          this.coverImgDir.length - 1
        ];
      }
    },

    inited(viewer) {
      this.$viewer = viewer;
    }
  }
};
</script>

<style>
table.v-table thead tr {
  color: red !important;
}
tbody tr:nth-of-type(odd) {
  background-color: rgba(0, 0, 0, 0.05);
}

.club-info-text-area-label {
  font-size: 14px;
  color: #a3a3a3;
  font-weight: 400;
  margin-bottom: 7px;
  display: inline-block;
}
</style>
