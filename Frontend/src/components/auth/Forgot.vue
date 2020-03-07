<template>
  <v-content>
    <v-container
      fill-height
      fluid
    >
      <v-snackbar
        v-model="snackbar"
        top
        right
        :timeout="3000"
        :color="snackColor"
      >
        {{ message }}
        <v-btn
          flat
          @click="snackbar = false"
        >
          Close
        </v-btn>
      </v-snackbar>
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
                Reset Password
              </v-toolbar-title>
              <v-spacer />
            </v-toolbar>
            <v-card-text>
              <v-form>
                <v-text-field
                  ref="email"
                  v-model="email"
                  :rules="[() => !!email || 'This field is required']"
                  prepend-icon="mdi-account"
                  label="Email"
                  required
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
                @click="reset"
              >
                Reset
              </v-btn>
              <v-btn
                align-center
                justify-center
                color="general"
                to="login"
              >
                Back
              </v-btn>
            </v-card-actions>
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
      email: "",
      message: "",
      snackbar: false,
      snackColor: "general"
    };
  },
  methods: {
    reset: function() {
      let email = this.email;
      if (email !== "") {
        this.$store
          .dispatch("RESET", email)
          .then(response => {
            this.snackbar = true;
            this.snackColor = "success";
            this.message =
              "Email sent to " + this.email + ". Please check your email";
            this.email = "";
          })
          .catch(err => {
            console.log(err);
            this.snackbar = true;
            this.snackColor = "error";
            this.message =
              "Email not sent to the address. Please try again later";
          });
      } else {
        this.snackbar = true;
        this.snackColor = "error";
        this.message = "Please enter your email";
      }
    }
  },
  metaInfo() {
    return {
      title: "Whiteboard | Forgot Password"
    };
  }
};
</script>
