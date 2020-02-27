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
                Whiteboard LMS
              </v-toolbar-title>
              <v-spacer />
            </v-toolbar>
            <v-card-text>
              <v-form>
                <v-text-field
                  ref="username"
                  v-model="username"
                  :rules="[() => !!username || 'This field is required']"
                  prepend-icon="mdi-account"
                  label="Login"
                  required
                />
                <v-text-field
                  ref="password"
                  v-model="password"
                  :rules="[() => !!password || 'This field is required']"
                  :append-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                  :type="showPassword ? 'text' : 'password'"
                  prepend-icon="mdi-lock"
                  label="Password"
                  counter
                  required
                  @keydown.enter="login"
                  @click:append="showPassword = !showPassword"
                />
              </v-form>
            </v-card-text>
            <v-divider class="mt-5" />
            <v-card-actions>
              <v-flex xs12>
                <v-layout
                  justify-space-between
                  row
                >
                  <v-btn
                    align-center
                    justify-center
                    flat
                    to="./forgot"
                  >
                    Forget Password
                  </v-btn>
                  <v-spacer />
                  <v-btn
                    align-center
                    justify-center
                    color="general"
                    @click="login"
                  >
                    Login
                  </v-btn>
                </v-layout>
              </v-flex>
            </v-card-actions>
            <v-snackbar
              v-model="snackbar"
              :color="color"
              :top="true"
            >
              {{ errorMessages }}
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

<script>
export default {
  data: function() {
    return {
      username: "",
      password: "",
      errorMessages: "Incorrect login info",
      snackbar: false,
      color: "general",
      showPassword: false
    };
  },
  // Sends action to Vuex that will log you in and redirect to the dash otherwise, error
  methods: {
    login: function() {
      let username = this.username;
      let password = this.password;
      this.$store
        .dispatch("LOGIN", { username, password })
        .then(() => this.$router.push("/dashboard"))
        .catch(err => {
          this.snackbar = true;
          this.snackColor = "error";
          this.message = "Login Error, Please try again later";
          console.log(err);
        });
    }
  },
  metaInfo() {
    return {
      title: "Whiteboard | Login"
    };
  }
};
</script>
