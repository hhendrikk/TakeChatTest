<template>
  <chat />
</template>

<script>

import {defineComponent} from "vue";
import Chat from "./components/Chat.vue";

export default defineComponent({
  name: 'App',
  components: { Chat }
})
</script>

<style>
html,
body,
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;

  width: 100%;
  height: 100%;

  margin: 0;
  padding: 0;
  overflow: hidden;
}
</style>
