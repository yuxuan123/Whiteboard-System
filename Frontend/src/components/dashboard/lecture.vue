<template>
  <v-container
    fluid
    grid-list-xl
    fill-height
    class="pa-0"
  >
    <v-layout
      v-if="liveLectureStatus"
      row
      wrap
    >
      <v-flex
        xs12
        sm12
        md4
        class="pa-0"
      >
        <v-card max-height="100%">
          <v-card-text class="pa-2 ml-1">
            <v-flex
              row
              class="scroll-y"
              style="height: 47vh;"
            >
              <v-list
                subheader
                two-line
              >
                <v-flex
                  style="position: sticky;top: 0;z-index: 999;background-color:#ffffff;"
                >
                  <h4 class="mt-0">
                    {{ content.title }}
                  </h4>
                  <v-text-field
                    v-model="newNote"
                    counter="50"
                    maxlength="50"
                    hint="Press enter to submit"
                    label="Add key notes..."
                    @keyup.enter="addNote"
                  />
                </v-flex>

                <v-list-tile
                  v-for="note in notes"
                  :key="note.title"
                  ripple
                  @click="addNote"
                >
                  <v-list-tile-content>
                    <v-list-tile-title>{{ note.title }}</v-list-tile-title>
                  </v-list-tile-content>

                  <v-list-tile-action>
                    <v-btn
                      icon
                      @click="deleteNote(note)"
                    >
                      <v-icon color="grey">
                        delete
                      </v-icon>
                    </v-btn>
                  </v-list-tile-action>
                </v-list-tile>
              </v-list>
            </v-flex>
            <v-divider />
            <v-flex
              row
              style="height: 50vh;"
            >
              <v-flex
                ref="chatBox"
                row
                class="chatBox"
              >
                <template>
                  <v-flex
                    v-for="(message, index) in messages"
                    :key="index"
                    class="message py-1"
                    :class="{ own: message.userName == username }"
                  >
                    <v-flex
                      v-if="
                        index > 0 &&
                          messages[index - 1].user != message.userName
                      "
                      class="username"
                    >
                      {{ message.userName }}
                    </v-flex>
                    <v-flex
                      v-if="index == 0"
                      class="username"
                    >
                      {{ message.userName }}
                    </v-flex>
                    <v-flex class="content">
                      <div v-html="message.message" />
                    </v-flex>
                  </v-flex>
                </template>
              </v-flex>
              <v-flex>
                <v-textarea
                  v-model="newMessage"
                  append-outer-icon="send"
                  class="mx-2"
                  label="Message to send"
                  rows="1"
                  @keyup="inputKey"
                  @click:append-outer="addComment()"
                />
              </v-flex>
            </v-flex>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex
        xs12
        sm12
        md8
        class="pa-0"
      >
        <v-card max-height="100%">
          <v-card-text>
            <v-layout column>
              <video-player
                ref="videoPlayer"
                class="vjs-custom-skin"
                :options="playerOptions"
                :playsinline="true"
                @ready="playerReadied"
              />
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout
      v-else
      row
      wrap
      style="background-color:#303030; max-height:100% !important;"
    >
      <LiveLectureEnded />
    </v-layout>
  </v-container>
</template>

<script>
//Live chat library
import Pusher from "pusher-js";
import moment from "moment";
// custom skin css
import "../../styles/custom/video-theme.css";
import LiveLectureEnded from "../error/LiveLectureEnded.vue";
export default {
  components: { LiveLectureEnded: LiveLectureEnded },
  data() {
    return {
      liveLectureStatus: false,
      content: {},
      contents: [],
      // videojs options
      playerOptions: {
        autoplay: false,
        muted: false,
        language: "en",
        playbackRates: [0.5, 1.0, 1.5, 2.0],
        sources: [
          {
            type: "video/mp4",
            src:
              "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4"
          }
        ],
        poster: require("@/assets/default/video.png")
      },
      newNote: "",
      notes: [
        {
          id: "1",
          title: "This is note 1"
        },
        {
          id: "2",
          title: "This is note 2"
        },
        {
          id: "3",
          title: "This is note 3"
        }
      ],
      newMessage: "",
      messages: [],
      username: ""
    };
  },
  created() {
    //Check whether if there's any live lectures on going
    //Get userid & username
    let userId = $cookies.get("userid");
    this.username = $cookies.get("username");
    this.$store.dispatch("GETUSERCONTENT", userId).then(response => {
      //Check each and every content
      //See if current date time is before 2hours of content date time
      //If yes = live lecture
      for (var i = 0; i < response.data.length; i++) {
        if (response.data[i].type == "lecture") {
          this.contents.push(response.data[i]);
        }
      }
      //Check if there's any live lectures going on.
      //If there isn't, display no live lectures page
      if (this.contents.length > 0) {
        //By default it will load the first live lecture
        this.liveLectureStatus = true;
        this.content = this.contents[0];
        this.liveChatConfigs();
      }
    });
  },
  methods: {
    liveChatConfigs() {
      //Subscribe to the channel
      this.subscribeChannel(this.content.contentId);
      this.getAllChat(this.content.contentId);
      this.playerOptions.sources[0].src = this.content.url;
    },
    subscribeChannel(lectureId) {
      var pusher = new Pusher("0a3b3bc361a655ea56ac", {
        cluster: "ap1",
        forceTLS: true
      });
      pusher.subscribe(lectureId);
      pusher.bind("my-event", data => {
        this.messages.push(data);
        this.scrollTo();
      });
    },
    getAllChat(lectureId) {
      this.$store.dispatch("GETALLCHAT", lectureId).then(response => {
        this.messages = response.data;
      });
    },
    // player is ready
    playerReadied(player) {
      player.currentTime(0);
    },
    addNote() {
      var item = {
        id: Math.random(),
        title: this.newNote
      };
      this.notes.push(item);
      this.newNote = "";
    },
    deleteNote(note) {
      var id = note.id;
      for (var i = 0; i < this.notes.length; i++)
        if (this.notes[i].id === note.id) {
          this.notes.splice(i, 1);
          break;
        }
    },
    inputKey: function(event) {
      if (event.key == "Enter") {
        this.addComment();
      }
    },
    addComment() {
      var comment = {
        lectureId: this.content.contentId,
        userName: this.username,
        message: this.newMessage,
        dateTime: moment(new Date())
      };
      this.$store.dispatch("SENDPUSHERMESSAGE", comment).then(response => {
        this.newMessage = "";
        this.scrollTo();
      });
    },
    scrollTo() {
      this.$nextTick(() => {
        let currentHeight = this.$refs.chatBox.scrollHeight;
        var container = this.$el.querySelector(".chatBox");
        container.scrollTop = currentHeight;
      });
    }
  }
};
</script>

<style>
.video-js {
  position: relative !important;
  height: 99vh !important;
}

.vjs-poster {
  position: absolute !important;
  left: 0;
  right: 0;
  top: 0;
  bottom: 0;
}

.chatBox {
  box-sizing: border-box;
  height: 40vh;
  overflow-y: scroll;
}

.chatBox .username {
  font-size: 16px;
  font-weight: thin;
}

.chatBox .content {
  max-width: 50%;
  border-radius: 10px;
  display: inline-block;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.2), 0 1px 1px 0 rgba(0, 0, 0, 0.14),
    0 2px 1px -1px rgba(0, 0, 0, 0.12);
  word-wrap: break-word;
}

.message.own {
  text-align: right;
}
.message.own .content {
  background-color: #168ffd;
  color: white;
}
</style>
