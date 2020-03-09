<template>
  <v-content>
    <v-container fluid fill-height grid-list-xl>
      <v-layout row warp>
        <v-flex xs4 sm8 align-center justify-center>
          <header style="font-size:50px;color:black;">Whiteboard</header>
          <v-spacer />
          <v-row class="text-xs">
            <h3 style="font-size:40px;color:Black;">Welcome Back</h3>
          </v-row>
          <v-spacer />

          <v-row class="text-xs">
            <h3 style="font-size:20px;color:grey;">Please log in to your account</h3>
          </v-row>
          <v-card-text>
            <v-flex>
              <v-text-field
                outlined
                single-line
                ref="username"
                v-model="username"
                :rules="[() => !!username || 'This field is required']"
                prepend-icon="mdi-account"
                label="Login"
                required
              />
              <v-text-field
                outlined
                color="teal"
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
            </v-flex>
          </v-card-text>
          <v-divider class="mt-5" />

          <v-layout class="text-xs-right">
            <v-flex xs12>
              <v-btn flat to="./forgot" color="general">Forget Password</v-btn>
            </v-flex>
          </v-layout>
          <v-spacer />

          <v-spacer />
          <v-btn xs6 align-center justify-center color="general" @click="login" block>Login</v-btn>
          <v-layout class="text-xs-center">
            <v-flex xs12>
              <v-btn
                flat
                align-center
                justify-center
                to="./forgot"
                color="general"
              >Dont have an account? Sign Up</v-btn>
            </v-flex>
          </v-layout>
          <v-snackbar v-model="snackbar" :color="color" :top="true">
            {{ errorMessages }}
            <v-btn dark flat @click="snackbar = false">Close</v-btn>
          </v-snackbar>
        </v-flex>
        <v-flex xs8>
          <v-img
            src="https://www.extron.com/img/mktg/open_graph/ntu.jpg"
            aspect-ratio="1"
            height="100%"
          ></v-img>
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
