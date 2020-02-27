<template>
  <v-container
    fluid
    grid-list-xl
    fill-height
  >
    <v-layout
      justify-center
      align-center
    >
      <v-flex xs12>
        <material-card
          color="general"
        >
          <div
            slot="header"
          >
            <div class="title font-weight-light mb-2">
              Announcements
            </div>
            <div class="category">
              Check your tasks here! 
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
                <material-notification
                  class="mb-4"
                  color="success"
                  icon="mdi-tag"
                >
                  This is a plain notification
                </material-notification>
                <material-notification
                  class="mb-4"
                  color="warning"
                  icon="mdi-tag"
                >
                  This is a plain notification
                </material-notification>
                <material-notification
                  class="mb-4"
                  color="error"
                  icon="mdi-tag"
                >
                  This is a plain notification
                </material-notification>
                                <material-notification
                  class="mb-4"
                  color="purple"
                  icon="mdi-tag"
                >
                  This is a plain notification
                </material-notification>
                                <material-notification
                  class="mb-4"
                  color="general"
                  icon="mdi-tag"
                >
                  This is a plain notification
                </material-notification>
              </v-flex>
            </v-layout>
          </v-card-text>
        </material-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>