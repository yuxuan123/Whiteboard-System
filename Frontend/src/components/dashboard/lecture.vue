<template>
  <v-container
    fluid
    grid-list-xl
    fill-height
    class="pa-0"
  >
    <v-layout
      row
      wrap
    >
      <v-flex
        xs12
        sm12
        md4
        class="pa-0"
      >
        <v-card height="100%">
          <v-card-text class="pa-2 ml-1">
            <v-flex
              row
              class="scroll-y"
              style="height: 50vh;"
            >
              <v-list
                subheader
                two-line
              >
                <v-flex
                  style=" position: sticky;top: 0;z-index: 999;background-color:#ffffff;"
                >
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
                    :key="message.id"
                    class="message py-1"
                    :class="{ own: message.user == username }"
                  >
                    <v-flex
                      v-if="
                        index > 0 && messages[index - 1].user != message.user
                      "
                      class="username"
                    >
                      {{ message.user }}
                    </v-flex>
                    <v-flex
                      v-if="index == 0"
                      class="username"
                    >
                      {{ message.user }}
                    </v-flex>
                    <v-flex class="content">
                      <div v-html="message.content" />
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
        <v-card height="100%">
          <v-card-text>
            <v-layout column>
              <video-player
                ref="videoPlayer"
                class="vjs-custom-skin"
                :options="playerOptions"
                :playsinline="true"
                @play="onPlayerPlay($event)"
                @pause="onPlayerPause($event)"
                @ended="onPlayerEnded($event)"
                @loadeddata="onPlayerLoadeddata($event)"
                @waiting="onPlayerWaiting($event)"
                @playing="onPlayerPlaying($event)"
                @timeupdate="onPlayerTimeupdate($event)"
                @canplay="onPlayerCanplay($event)"
                @canplaythrough="onPlayerCanplaythrough($event)"
                @ready="playerReadied"
                @statechanged="playerStateChanged($event)"
              />
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
// custom skin css
import "../../styles/custom/video-theme.css";

export default {
  data() {
    return {
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
        },
        {
          id: "4",
          title: "This is note 4"
        },
        {
          id: "5",
          title: "This is note 5"
        }
      ],
      newMessage: "",
      messages: [
        {
          id: "1",
          user: "Alan",
          content: "hello there"
        },
        {
          id: "2",
          user: "Bill",
          content: "hiiii"
        },
        {
          id: "3",
          user: "Charlie",
          content: "heyyy"
        },
        {
          id: "4",
          user: "Dohn",
          content: "hello there"
        },
        {
          id: "5",
          user: "Ethan",
          content: "hiiii"
        },
        {
          id: "6",
          user: "Fanny",
          content: "heyyy"
        }
      ],
      username: "Bill",
    };
  },
  computed: {
    player() {
      return this.$refs.videoPlayer.player;
    }
  },
  mounted() {
    // console.log('this is current player instance object', this.player)
    setTimeout(() => {
      console.log("dynamic change options", this.player);
      // change src
      // this.playerOptions.sources[0].src = 'https://cdn.theguardian.tv/webM/2015/07/20/150716YesMen_synd_768k_vp8.webm';
      // change item
      // this.$set(this.playerOptions.sources, 0, {
      //   type: "video/mp4",
      //   src: 'https://cdn.theguardian.tv/webM/2015/07/20/150716YesMen_synd_768k_vp8.webm',
      // })
      // change array
      // this.playerOptions.sources = [{
      //   type: "video/mp4",
      //   src: 'https://cdn.theguardian.tv/webM/2015/07/20/150716YesMen_synd_768k_vp8.webm',
      // }]
      this.player.muted(false);
    }, 5000);
  },
  methods: {
    // listen event
    onPlayerPlay(player) {
      // console.log('player play!', player)
    },
    onPlayerPause(player) {
      // console.log('player pause!', player)
    },
    onPlayerEnded(player) {
      // console.log('player ended!', player)
    },
    onPlayerLoadeddata(player) {
      // console.log('player Loadeddata!', player)
    },
    onPlayerWaiting(player) {
      // console.log('player Waiting!', player)
    },
    onPlayerPlaying(player) {
      // console.log('player Playing!', player)
    },
    onPlayerTimeupdate(player) {
      // console.log('player Timeupdate!', player.currentTime())
    },
    onPlayerCanplay(player) {
      // console.log('player Canplay!', player)
    },
    onPlayerCanplaythrough(player) {
      // console.log('player Canplaythrough!', player)
    },
    // or listen state event
    playerStateChanged(playerCurrentState) {
      // console.log('player current update state', playerCurrentState)
    },
    // player is ready
    playerReadied(player) {
      // seek to 10s
      console.log("Player readied", player);
      player.currentTime(10);
      // console.log('example 01: the player is readied', player)
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
      var item = {
        id: Math.random(),
        user: this.username,
        content: this.newMessage
      };
      this.messages.push(item);
      this.newMessage = "";
      this.scrollTo();
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
  height: 100vh !important;
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
  background-color: #e6e5eb;
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
