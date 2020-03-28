<template>
  <v-app
    id="reset"
    dark
  >
    <template>
      <v-content>
        <v-container
          fill-height
          fluid
        >
          <v-layout
            align-center
            justify-center
          >
            <v-flex
              xs12
              sm8
              md4
            >
              <v-card class="elevation-12">
                <v-toolbar color="general">
                  <v-toolbar-title style="color:white!important;">
                    {{ header }}
                  </v-toolbar-title>
                  <v-spacer />
                </v-toolbar>
                <v-card-text>
                  <v-form>
                    <v-text-field
                      ref="password"
                      v-model="newpassword"
                      :rules="[() => !!newpassword || 'This field is required']"
                      :append-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                      :type="showPassword ? 'text' : 'password'"
                      prepend-icon="mdi-lock"
                      label="New Password"
                      counter
                      required
                      @click:append="showPassword = !showPassword"
                    />
                    <v-text-field
                      ref="password"
                      v-model="cfmpassword"
                      :rules="[() => !!cfmpassword || 'This field is required']"
                      :append-icon="showCfmPassword ? 'mdi-eye-off' : 'mdi-eye'"
                      :type="showCfmPassword ? 'text' : 'password'"
                      prepend-icon="mdi-lock"
                      label="Confirm Password"
                      counter
                      required
                      @keydown.enter="changePassword"
                      @click:append="showCfmPassword = !showCfmPassword"
                    />
                  </v-form>
                </v-card-text>
                <v-divider class="mt-5" />
                <v-card-actions>
                  <v-spacer />
                  <v-btn
                    align-center
                    justify-center
                    color="general"
                    @click="changePassword"
                  >
                    Change Password
                  </v-btn>
                </v-card-actions>
                <v-snackbar
                  v-model="snackbar"
                  :color="color"
                  :top="true"
                >
                  {{ messages }}
                  <v-btn
                    dark
                    flat
                    @click="snackbar = false"
                  >
                    Close
                  </v-btn>
                </v-snackbar>
              </v-card>
            </v-flex>
          </v-layout>
        </v-container>
      </v-content>
    </template>
  </v-app>
</template>

<script>
export default {
  data: function() {
    return {
      title: "",
      header: "",
      username: "",
      password: "",
      newpassword: "",
      cfmpassword: "",
      snackbar: false,
      color: "general",
      messages: "",
      showPassword: false,
      showCfmPassword: false
    };
  },
  created: function() {
    var route = this.$route.path.split("/");
    var url = route[1];
    if (url === "resetpassword") {
      this.title = "Whiteboard | Reset Password";
      this.header = "Reset Password";
    } else if (url === "activate") {
      this.title = "Whiteboard | Activate Account";
      this.header = "Set Password";
    }
  },
  methods: {
    changePassword() {
      var cur= this;
       var stringURL= this.$route.params.userId;
      stringURL = stringURL.replace("class=", "");
      this.userId=stringURL.trim();
      
      if (this.newpassword !== this.cfmpassword) {
        this.snackbar = true;
        this.color = "error";
        this.messages = "Password does not match!";
      } else {
        this.$store
          .dispatch("SETNEWPASSWORD", {
            newpassword: cur.newpassword,
            userid: this.userId
          })
          .then(success => {
            this.snackbar = true;
            this.color = "success";
            this.messages = success.data;
            this.newpassword = "";
            this.cfmpassword = "";
            this.$store.dispatch("LOGOUT").then(() => {
              this.$router.push("/");
            });
          })
          .catch(error => {
            console.log(error);
            this.snackbar = true;
            this.color = "error";
            this.messages = "Wrong password!";
          });
      }
    }
  },
  metaInfo() {
    return {
      title: this.title
    };
  }
};
</script>
