<template>
  <v-content>
    <v-container
      pa-0
      fluid
      fill-height
    >
      <v-layout
        row
        wrap
      >
        <v-flex
          xs12
          sm6
          class="active"
        >
          <v-layout
            align-center
            justify-center
            fill-height
            pa-5
          >
            <v-layout
              column
            >
              <v-flex
                xs12
                mb-3
              >
                <div class="text-xs-center">
                  <v-img
                    :src="logo"
                    height="70"
                    contain
                  />
                  <div class="headline mt-2">
                    Please login to your account
                  </div>           
                </div>
              </v-flex>
              <v-flex
                xs12
                class="login-form"
              >
                <v-form>
                  <v-text-field
                    v-model="username"
                    box
                    full-width
                    single-line
                    label="Username"
                    background-color="#f4f8f7"
                    color="grey darken-2"
                    prepend-inner-icon="mdi-account-outline"
                    mb-0
                  />
                  <v-text-field
                    v-model="password"
                    :append-icon="show ? 'mdi-eye-outline' : 'mdi-eye-off-outline'"
                    :type="show ? 'text' : 'password'"
                    box
                    full-width
                    single-line
                    label="Password"
                    background-color="#f4f8f7"
                    color="grey darken-2"
                    prepend-inner-icon="mdi-lock-outline"
                    autocomplete="password"
                    @click:append="show = !show" 
                  />
                </v-form>
              </v-flex>
              <span style="text-align: right !important;"><a href="/forgot">Forget password?</a></span>
              <div class="text-xs-center">
                <v-btn
                  round
                  dark
                  ripple
                  color="teal"
                  class="mt-3 mb-3"
                  style="width:200px;"
                  @click="login"
                >
                  Log in
                </v-btn>
              </div>
              <span style="text-align: center;">Don't have an account?<a href="/signup"> Sign up!</a></span>
            </v-layout>
          </v-layout>
        </v-flex>
        <v-flex
          xs12
          sm6
          class="login-bg"
        >
          <v-layout
            column
            align-center
            justify-center
            fill-height
            pa-3
          >
            <div
              class=" text-xs-center mb-3 pa-4"
              style="background-color:rgba(0, 0, 0, 0.5);"
            >
              <div class="display-1 font-weight-black mb-3">
                Welcome Back! 
              </div>
              <span class="subheading">To keep connected with us, please login with your school info</span>
            </div>
          </v-layout>
        </v-flex>
      </v-layout>
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
    </v-container>
  </v-content>
</template>

<script>
export default {
  data: function() {
    return {
      logo: require("@/assets/default/iconwhite.png"),
      show: false,
      username: "",
      password: "",
      errorMessages: "Incorrect login info",
      snackbar: false,
      color: "general",
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
          this.color = "error";
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

<style>
.login-bg {
    min-height: 530px;
    background-image: url("../../assets/default/hive.jpg");
    background-repeat: no-repeat;
    background-size: cover;
    color: white;
  }

  .login-form {
    min-width: 50%;
  }

  .active {
    background: #fff;
    color: #40ae9f;
  }

  .login-form .v-input__control > .v-input__slot {
    background: rgba(244,248,247,1);
  }

  .login-form .v-text-field.v-text-field--enclosed .v-text-field__details {
    margin-bottom: 0px;
    height: 0px;
  }
</style>