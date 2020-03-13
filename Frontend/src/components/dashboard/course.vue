<template>
  <v-layout>
    <h1>Pusher Test</h1>
    <p>
      Publish an event to channel <code>my-channel</code> with event name
      <code>my-event</code>; it will appear below:
    </p>
    <v-flex>
      <ul>
        <li v-for="message in messages" :key="message.id">
          {{ message }}
        </li>
      </ul>
    </v-flex>
  </v-layout>
</template>

<script>
import Pusher from "pusher-js";

Pusher.logToConsole = true;

var pusher = new Pusher("0a3b3bc361a655ea56ac", {
  cluster: "ap1",
  forceTLS: true
});

export default {
  data: () => ({
    messages: []
  }),
  created() {
    this.subscribe();
    this.sendMessage();
  },
  methods: {
    subscribe() {
      var pusher = new Pusher("0a3b3bc361a655ea56ac", {
        cluster: "ap1",
        forceTLS: true
      });
      pusher.subscribe("my-channel");
      pusher.bind("my-event", data => {
        this.messages.push(JSON.stringify(data));
      });
    },
    sendMessage() {
      this.axios({
        method: "post",
        url: "https://whiteboardsyetem.azurewebsites.net/pusher",
        data: {
          name: "Alan",
          message: "Test"
        },
        config: { headers: { "Content-Type": "application/json" } }
      }).then(response => {
        console.log("sent");
      });
    }
  }
};
</script>
