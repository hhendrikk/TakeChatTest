<template>
  <div class="container">
    <div class="left">
      <div class="channels">
        <div class="title">
          <h2>Channels</h2>
          <button @click="createChannelVisible = !createChannelVisible">+</button>
        </div>
        <div class="channels-inner">
          <div @click="onSelectChannel(channel)" class="item" :class="{selected: channel === channelSelected}"
               v-for="channel in channels" :key="channel">
            <span>{{ channel }}</span>
          </div>
        </div>
      </div>
      <div class="users">
        <div class="title">
          <h2>Users</h2>
        </div>
        <div class="users-inner" v-if="chatUsers.length">
          <div @click="onSelectUser(user)" class="item"
               :class="{selected: userSelected && user.id === userSelected.id}"
               v-for="user in chatUsers" :key="user.id">
            <span>{{ user.nickname }}</span>
            <span>{{ chatPrivate.filter(x => x.sender.nickname === user.nickname).length }}</span>
          </div>
        </div>
      </div>
      <div class="logout">
        <button @click="onLogout">Logout</button>
      </div>
    </div>
    <div class="content">
      <div class="chat">
        <span :key="`chat_channel_${index}`" v-for="(chat, index) in chatChannel">
          <span class="chat-nickname" v-if="chat.nickname !== nickname && chat.type < 5">{{ chat.nickname }}: </span>
          <span class="chat-nickname" v-if="chat.nickname === nickname && chat.type < 5">You: </span>
          {{ chat.message }}
        </span>

        <div v-if="userSelected" style="display: flex; flex-direction: column">
          <span :key="`chat_private_${index}`" v-for="(chat, index) in chatPrivate">
            <span class="chat-nickname" v-if="chat.sender.nickname === nickname">You: </span>
            <span class="chat-nickname" v-if="chat.receive.nickname === nickname">{{ chat.receive.nickname }}: </span>
            {{ chat.message }}
          </span>
        </div>
      </div>
      <input v-model="message" type="text" @keydown.enter="onSendMessage">
    </div>
  </div>

  <teleport to="body">
    <div class="user-form" :class="{hidden: userFormHidden}">
      <div class="nickname">
        <div class="logon">CHAT</div>

        <div class="nickname-inner">
          <span>Nickname:</span>
          <input v-model="nickname" type="text"/>
        </div>

        <div class="option">
          <span>{{ messageRequired }}</span>
          <button @click="onJoin">Enter</button>
        </div>
      </div>
    </div>
  </teleport>

  <teleport to="body">
    <div class="channel-form" v-if="createChannelVisible">
      <div class="channel">
        <div class="channel-inner">
          <span>Channel:</span>
          <input v-model="channelNew" type="text"/>
        </div>

        <div class="option">
          <button @click="createChannelVisible = !createChannelVisible">Fechar</button>
          <button @click="onChannelAdd">Add</button>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
import {defineComponent, ref, computed} from "vue";

export default defineComponent({
  name: "Chat",
  setup() {
    const uriWebSocket = "ws://localhost:5000/ws/{nickname}";
    const uriApi = "http://localhost:5000/api/users/{nickname}";
    const nickname = ref('');
    const userFormHidden = ref(nickname.value !== '');
    const websocket = ref();
    const messageRequired = ref();
    const channels = ref([]);
    const user = ref();
    const users = ref([]);
    const message = ref();
    const channelNew = ref();
    const createChannelVisible = ref(false);

    const messagesChannel = ref([]);
    const messagesPrivate = ref([]);

    const channelSelected = ref("#general");
    const userSelected = ref();

    const chatChannel = computed(() => messagesChannel.value.filter(x => x.channel === channelSelected.value || x.type === 1))

    const chatPrivate = computed(() => messagesPrivate.value.filter(x =>
        x.sender.nickname === nickname.value
        || x.receive.nickname === nickname.value))

    const chatUsers = computed(() => users.value.filter(x => x.nickname !== nickname.value));

    async function onJoin() {
      if (!nickname.value) {
        messageRequired.value = "Nickname is required!";
        return;
      }

      const existsNickname = await fetch(uriApi.replace("{nickname}", nickname.value)).then(data => data.json());

      if (existsNickname) {
        messageRequired.value = "Nickname is already used!";
        return;
      }

      websocket.value = new WebSocket(uriWebSocket.replace("{nickname}", nickname.value));

      websocket.value.onopen = () => {
        console.log("opened connection!");
      };

      websocket.value.onclose = () => {
        console.log("closed connection!");
      };

      websocket.value.onmessage = event => {
        const command = JSON.parse(event.data);

        if (command.type === 1) {
          if (!channels.value.filter(c => c === `#${command.channel}`).length) {
            channels.value = [...channels.value, `#${command.channel}`];
          }

          messagesChannel.value = [
            {
              message: command.message,
              channel: `#${command.channel}`,
              nickname: command.sender.nickname,
              type: command.type,
            }
          ];
        }

        if (command.type === 2 || command.type === 4) {
          messagesChannel.value = [
            ...messagesChannel.value,
            {
              nickname: command.sender.nickname,
              message: command.message,
              channel: `#${command.channel}`,
              type: command.type,
            }];
        }

        if (command.type === 6) {
          messagesChannel.value = [
            ...messagesChannel.value,
            {
              nickname: command.sender.nickname,
              message: command.message,
              channel: `#${command.channel}`,
              type: command.type,
            }];

          users.value = users.value.filter(x => x.nickname !== command.sender.nickname);
        }

        if (command.type === 3) {
          messagesPrivate.value = [
            ...messagesPrivate.value,
            {
              sender: command.sender,
              receive: command.receive,
              message: command.message,
              type: command.type,
            }];
        }

        if (command.type === 5) {
          if (!channels.value.filter(c => c === `#${command.channel}`).length) {
            channels.value = [...channels.value, `#${command.channel}`];
          }

          if (nickname.value === command.sender.nickname) {
            user.value = command.sender;
          }

          messagesChannel.value = [
            ...messagesChannel.value,
            {
              nickname: command.sender.nickname,
              message: command.message,
              channel: `#${command.channel}`,
              type: command.type,
            }]
        }

        if (command.type === 7) {
          users.value = command.users;
        }
      };

      userFormHidden.value = true;
    }

    function onSendMessage() {
      let command = {};

      if (channelSelected.value) {
        command = {
          type: 2,
          message: message.value,
          sender: user.value,
          channel: channelSelected.value.replace('#', '')
        }
      } else {
        command = {
          type: 3,
          message: message.value,
          sender: user.value,
          receive: userSelected.value,
        }
      }

      websocket.value.send(JSON.stringify(command));
      message.value = "";
    }

    function onChannelAdd() {
      const command = {
        type: 1,
        sender: user.value,
        channel: channelNew.value
      };

      websocket.value.send(JSON.stringify(command));
      channelSelected.value = `#${channelNew.value}`;
      createChannelVisible.value = false;
    }

    function onSelectChannel(channel) {
      channelSelected.value = channel;
      userSelected.value = null;

      const command = {
        type: 4,
        sender: user.value,
        channel: channel.replace('#', '')
      };

      websocket.value.send(JSON.stringify(command));
    }

    function onSelectUser(user) {
      userSelected.value = user;
      channelSelected.value = null;
    }

    function onLogout() {
      const command = {
        type: 6,
        sender: user.value,
        message: `${nickname.value} has disconnect!`,
        channel: "general",
      }

      websocket.value.send(JSON.stringify(command));

      setTimeout(() => {
        websocket.value.close();
        nickname.value = "";
        userFormHidden.value = false;
      }, 1000);
    }

    return {
      userFormHidden,
      nickname,
      messageRequired,
      chatChannel,
      chatPrivate,
      channels,
      channelSelected,
      message,
      userSelected,
      user,
      channelNew,
      createChannelVisible,
      messagesPrivate,
      chatUsers,
      onLogout,
      onSelectUser,
      onChannelAdd,
      onSendMessage,
      onJoin,
      onSelectChannel
    }
  }
})
</script>

<style scoped>

input {
  padding: 5px;
}

.logout {
  position: absolute;
  bottom: 10px;
  left: 40px;
}

.logout button {
  width: 100px;
  border: 0;
  background-color: dimgray;
  color: white;
  padding: 5px;
  cursor: pointer;
}

.item {
  padding: 10px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.04);
  cursor: pointer;

  display: flex;
  justify-content: space-between;
}

.item.selected {
  background-color: dodgerblue;
  color: white;
}

.user-form {
  position: absolute;
  display: flex;
  justify-content: center;
  align-items: center;
  left: 0;
  top: 0;

  background-color: #fff;

  width: 100%;
  height: 100%;

  transition: top 0.35s linear;
}

.channel-form {
  position: absolute;
  left: 0;
  top: 0;
  display: flex;
  justify-content: center;
  align-items: center;

  width: 100%;
  height: 100%;
}

.channel-form > .channel {
  width: 300px;
  height: 90px;

  padding: 10px;
  background-color: whitesmoke;
}

.channel-form > .channel > .channel-inner {
  display: flex;
  justify-content: center;
  align-items: center;

  height: 60px;
}

.channel-form > .channel > .option button {
  margin-right: 30px;
  width: 50px;
}

.channel-form > .channel > .channel-inner > span {
  padding-right: 10px;
}

.user-form.hidden {
  top: -100%;
}

.user-form > .nickname {
  display: flex;
  flex-direction: column;
  align-items: center;

  background-color: whitesmoke;
  width: 300px;
  height: 130px;

  padding: 10px;
  border-radius: 5px;
}

.channel-form > .channel > .option,
.user-form > .nickname > .option {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding-right: 40px;
  width: 100%;
}

.channel-form > .channel > .option button,
.user-form > .nickname > .option button {
  border: 0;
  background-color: dimgray;
  color: white;
  padding: 5px;
  cursor: pointer;
}

.user-form > .nickname > .option span {
  margin-right: 30px;
  color: lightcoral;
}

.user-form > .nickname > .nickname-inner span {
  margin-right: 10px;
}

.user-form > .nickname > .nickname-inner {
  margin: 20px;
  display: flex;

  align-items: center;
}

.user-form > .nickname > .logon {
  font-size: 2.5rem;
  font-weight: bold;
}

.container {
  display: grid;
  grid-template-columns: 200px auto;
  grid-template-rows: auto;
  grid-template-areas: "left content";

  width: 100%;
  height: 100%;
}

.container > div {
  width: 100%;
  height: 100%;
}

.container .left {
  grid-area: left;
  border-right: 1px solid rgba(0, 0, 0, 0.1);
}

.container .content {
  grid-area: content;
  display: grid;

  grid-template-rows: auto 40px;
  padding: 1px;
}

.container .content .chat {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);

  padding: 20px;
}

.container .content input {
  font-size: 1.2rem;
  border: 0;
  outline: 0;
  padding: 10px;
}


.container .content input:focus {
  outline: none !important;
}

.title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;

  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
  border-top: 1px solid rgba(0, 0, 0, 0.05);
}

.title button {
  width: 25px;
  height: 25px;

  cursor: pointer;

  border: 0;
  background-color: dimgray;
  opacity: 0.8;
  color: white;
  border-radius: 50%;

  font-size: 1.2rem;

  transition: opacity 0.25s linear;
}

.title button:hover {
  opacity: 1;
}

.users .title > h2,
.channels .title > h2 {
  font-size: 1.2rem;
  height: 1.2rem;
  margin: 0;
}

.users > .users-inner,
.channels > .channels-inner {
  height: 200px;
}

.chat-nickname {
  font-weight: bold;
}
</style>