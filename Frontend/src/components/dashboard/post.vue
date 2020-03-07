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
            <v-dialog v-model="discussionDialog" max-width="1000px">
              <v-card>
                <v-card-title>
                  <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                  <v-container class="pt-0" grid-list-md>
                    <v-layout wrap>
                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingDiscussion.username"
                          label="Name"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingDiscussion.email"
                          label="Email"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingDiscussion.phoneno"
                          label="Phone Number"
                          readonly
                        />
                      </v-flex>

                      <v-flex xs12 sm6 md6>
                        <v-text-field
                          v-model="viewingDiscussion.role"
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
                      @click="closeviewingDiscussion"
                    >
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
                            v-model="editedDiscussion.username"
                            label="Name"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="editedDiscussion.email"
                            label="Email"
                            :readonly="isViewing"
                          />
                        </v-flex>
                        <v-flex xs12 sm6 md6>
                          <v-text-field
                            v-model="editedDiscussion.phoneno"
                            label="Phone No"
                            :readonly="isViewing"
                          />
                        </v-flex>

                        <v-flex xs6 sm6 md6>
                          <v-text-field
                            v-model="editedDiscussion.role"
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
              :items="DiscussionList"
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
                <td>{{ props.item.username }}</td>
                <td>{{ props.item.email }}</td>
                <td>{{ props.item.phoneno }}</td>
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
      {{ errorMessages }}
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
    discussionDialog: false,
    search: "",
    pagination: {
      page: 1,
      rowsPerPage: 5,
      sortBy: "",
      descending: true,
      rowsPerPageItems: [5, 10, 15],
      totalItems: 0,
      search: "",
    },
    DiscussionList: [
      {
        postId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        courseId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        title: "alan@gmail.com",
        description: "Testing here 123",
        createdBy: "2020-03-04T06:29:03.565Z",
        createdOn: "2020-03-04T06:29:03.565Z"
      }
    ],
    dialog: false,
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
    editedDiscussion: {},
    viewingDiscussion: {},
    errorMessages: "",
    snackbar: false,
    color: "general"
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

    discussionDialog(val) {
      val || this.closeviewingDiscussion();
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
      this.$store
        .dispatch("GETALLUSER", this.pagination)
        .then(response => {
          console.log(response);
        })
        .catch(err => {
          this.snackbar = true;
          this.color = "error";
          this.message = "Error, Please try again later";
        });
      this.pagination.totalItems = this.DiscussionList.length;
    },

    editUser(user) {
      this.editedIndex = this.DiscussionList.indexOf(user);
      this.editedDiscussion = Object.assign({}, user);
      this.dialog = true;
    },

    viewUser(user) {
      this.editedIndex = -2;
      this.viewingDiscussion = Object.assign({}, user);
      this.discussionDialog = true;
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
        let userIndex = this.DiscussionList.findIndex(
          item => item.id === user.id
        );
        if (~userIndex) {
          this.DiscussionList.splice(userIndex, 1);
          this.loading = false;
        }
        this.pagination.totalItems = this.DiscussionList.length;
      }
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
        this.submitEditUser();
      } else {
        this.createUser();
      }
    },

    createUser() {
      this.DiscussionList.push(this.editedDiscussion);
      this.pagination.totalItems = this.DiscussionList.length;
      this.close();
    },

    submitEditUser() {
      let userIndex = this.DiscussionList.findIndex(
        user => user.id === this.editedDiscussion.id
      );

      console.log(userIndex);
      console.log(this.editedDiscussion);
      this.DiscussionList.splice(userIndex, 1, this.editedDiscussion);
      console.log(this.DiscussionList);
      this.close();

      this.loading = true;
    },

    searchUser(input) {
      if (input.length > 2) {
        //API Call
        console.log(input);
      }
    }
  }
};
</script>
