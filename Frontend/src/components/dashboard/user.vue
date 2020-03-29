<template>
  <v-container fill-height fluid grid-list-xl>
    <v-layout align-center justify-center wrap>
      <v-flex md12>
        <div>
          <material-card
            color="general"
            title="Admin Dashboard"
            text="Manage User Accounts"
          >
            <v-spacer />
            <br />
            <v-dialog v-model="userDialog" max-width="1000px">
              <v-card>
                <v-card-title>
                  <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                  <v-container class="pt-0" grid-list-md>
                    <v-layout wrap>
                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingUser.userName"
                          label="Name"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingUser.email"
                          label="Email"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingUser.phoneNo"
                          label="Phone Number"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
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
                    <v-btn color="blue darken-1" flat @click="closeViewingUser">
                      Close
                    </v-btn>
                  </v-card-actions>
                </v-card-text>
              </v-card>
            </v-dialog>
            <v-dialog v-model="dialog" max-width="1000px">
              <template v-slot:activator="{ on }">
                <v-btn color="general" dark v-on="on">
                  New User
                </v-btn>
              </template>

              <v-card>
                <v-card-text>
                  <v-card-title>
                    <span class="headline">{{ formTitle }}</span>
                  </v-card-title>
                  <v-form ref="createform">
                    <v-container class="pt-0" grid-list-md>
                      <v-layout wrap>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="editedUser.userName"
                            label="Name"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="editedUser.email"
                            label="Email"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="editedUser.phoneNo"
                            label="Phone No"
                            :readonly="isViewing"
                          />
                        </v-flex>

                        <v-flex xs6 sm6 md6>
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
                  <v-btn color="blue darken-1" flat @click="close">
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

            <v-flex xs12 sm12 md12>
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
              <template slot="headerCell" slot-scope="{ header }">
                <span
                  class="subheading font-weight-light text-general text--darken-3"
                  v-text="header.text"
                />
              </template>
              <template slot="items" slot-scope="props">
                <td>{{ props.item.userName }}</td>
                <td>{{ props.item.email }}</td>
                <td>{{ props.item.phoneNo }}</td>
                <td>{{ props.item.role }}</td>
                <td>
                  <v-layout align-center justify-center>
                    <v-card-actions>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="viewUser(props.item)"
                      >
                        <v-icon size="20" color="blue lighten-1">
                          mdi-eye
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="editUser(props.item)"
                      >
                        <v-icon size="20" color="blue lighten-1">
                          edit
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="emailUser(props.item)"
                      >
                        <v-icon size="20" color="blue lighten-1">
                          mdi-email
                        </v-icon>
                      </v-btn>
                      <v-btn
                        flat
                        icon
                        color="blue lighten-1"
                        @click="deleteUser(props.item)"
                      >
                        <v-icon size="20" color="blue lighten-1">
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
    <v-snackbar v-model="snackbar" :color="color" :top="true">
      {{ message }}
      <v-btn dark flat @click="snackbar = false">
        Close
      </v-btn>
    </v-snackbar>
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
      search: "",
      page: 1,
      rowsPerPage: 5,
      sortBy: "username",
      descending: true,
      rowsPerPageItems: [5, 10, 15],
      totalItems: 0
    },
    UserList: [],
    dialog: false,
    headers: [
      {
        text: "Username",
        value: "userName"
      },
      { text: "Email", value: "email" },
      {
        text: "Phone Number",
        value: "phoneNo",
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
    viewingUser: {},
    snackbar: false,
    color: "general",
    message: ""
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
        this.getUsers();
      },
      deep: true
    },

    search: {
      handler(input) {
        this.searchUser(input);
      }
    }
  },

  methods: {
    getUsers() {
      this.loading = true;
      //Pass Search value into pagination object
      this.pagination.search = this.search;
      this.$store
        .dispatch("GETALLUSER", this.pagination)
        .then(response => {
          this.loading = false;
          this.UserList = response.data.userDtos;
          var xpg = JSON.parse(response.headers["x-pagination"]);
          this.pagination.totalItems = xpg["totalCount"];
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
    },

    searchUser(input) {
      if (input.length > 3 || input.length == 0) {
        //API Call
        this.getUsers();
      }
    },

    viewUser(user) {
      this.editedIndex = -2;
      this.viewingUser = Object.assign({}, user);
      this.userDialog = true;
    },

    editUser(user) {
      this.editedIndex = this.UserList.indexOf(user);
      this.editedUser = Object.assign({}, user);
      this.dialog = true;
    },

    emailUser(user) {
      var answer = window.confirm("Send activation email?");
      if (answer) {
        this.$store
          .dispatch("RESET", user.email)
          .then(response => {
            this.snackbar = true;
            this.color = "success";
            this.message =
              "Email sent to " + user.email + ". Please check your email";
            this.email = "";
          })
          .catch(err => {
            console.log(err);
            this.snackbar = true;
            this.color = "error";
            this.message =
              "Email not sent to the address. Please try again later";
          });
      }
    },

    close() {
      this.editedUser = {};
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
      this.$store
        .dispatch("CREATEUSER", this.editedUser)
        .then(response => {
          this.UserList.push(this.editedUser);
          this.pagination.totalItems = this.UserList.length;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Create Error, Please try again later";
        });
      this.close();
    },

    submitEditUser() {
      //Save editedUser out to a temp var
      //To replace in the table after update
      var editedUser = this.editedUser;
      this.$store
        .dispatch("UPDATEUSER", this.editedUser)
        .then(response => {
          let userIndex = this.UserList.findIndex(
            user => user.userId === editedUser.userId
          );
          this.UserList.splice(userIndex, 1, editedUser);
          this.pagination.totalItems = this.UserList.length;
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Update Error, Please try again later";
        });
      this.close();
    },

    deleteUser(user) {
      var answer = window.confirm("Confirm delete user?");
      if (answer) {
        this.$store
          .dispatch("DELETEUSER", user)
          .then(response => {
            this.loading = true;
            let userIndex = this.UserList.findIndex(
              item => item.id === user.id
            );
            if (~userIndex) {
              this.UserList.splice(userIndex, 1);
              this.loading = false;
            }
            this.pagination.totalItems = this.UserList.length;
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
