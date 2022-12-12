<template>
  <div class="resources">
    <h5 class="mb-2 title">Resources</h5>
    <div
      class="tile mb-2 mr-2"
      :class="{ 'tile-active': resourcesInfoTiles[0] }"
      @click="toggleResourcesInfoTile(0)"
    >
      <div class="mb-2">
        <span class="font-medium text-xl">Deposit</span>
      </div>
      <div>
        <span class="text-bluegray-900 text-2xl">{{ portfolio.deposit }}</span>
      </div>
    </div>
    <div
      class="tile mb-2"
      :class="{ 'tile-active': resourcesInfoTiles[1] }"
      @click="toggleResourcesInfoTile(1)"
    >
      <div class="mb-2">
        <span class="font-medium text-xl">Cash on Hand</span>
      </div>
      <div>
        <span class="text-bluegray-900 text-2xl">{{
          portfolio.cashOnHand
        }}</span>
      </div>
    </div>
    <div
      class="tile"
      :class="{ 'tile-active': resourcesInfoTiles[2] }"
      @click="toggleResourcesInfoTile(2)"
    >
      <div class="mb-2">
        <span class="font-medium text-xl">Portfolio Value</span>
      </div>
      <div>
        <span class="text-bluegray-900 text-2xl">{{ portfolio.value }}</span>
      </div>
    </div>
    <div class="resources-crud">
      <EditDepositDialog
        :editDepositDialog="resourcesInfoTiles[0]"
        :portfolio="portfolio"
        @deposit-edited="handleDepositEdited($event)"
        @deposit-editing-canceled="handleDepositEditingCanceled()"
      />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import EditDepositDialog from "./EditDepositDialog.vue";

export default defineComponent({
  name: "PortfolioDashboard",

  components: {
    EditDepositDialog,
  },

  props: {
    portfolio: {
      type: Object,
      required: true,
      default: {},
    },
  },

  data() {
    return {
      resourcesInfoTiles: [false, false, false] as Array<Boolean>,
    };
  },

  methods: {
    toggleResourcesInfoTile(index: number) {
      for (let i = 0; i < this.resourcesInfoTiles.length; i++) {
        if (i == index) {
          continue;
        }
        this.resourcesInfoTiles[i] = false;
      }

      this.resourcesInfoTiles[index] = !this.resourcesInfoTiles[index];
    },

    handleDepositEdited(deposit: any) {
      if (deposit.deposit) {
        this.portfolio.deposit = String(
          Number(this.portfolio.deposit) + Number(deposit.check)
        );
      }

      if (deposit.withdraw) {
        this.portfolio.deposit = String(
          Number(this.portfolio.deposit) - Number(deposit.check)
        );
      }

      if (deposit.override) {
        this.portfolio.deposit = String(Number(deposit.check));
      }
      this.resourcesInfoTiles[0] = false;
    },

    handleDepositEditingCanceled() {
      this.resourcesInfoTiles[0] = false;
    }
  },
});
</script>

<style>
.tile {
  cursor: pointer;
  height: 7rem;
  width: 18.666rem;
  padding: 1rem;
  border-radius: 4px;
  box-shadow: 0 2px 1px -1px rgba(0, 0, 0, 0.2), 0 1px 1px 0 rgba(0, 0, 0, 0.14),
    0 1px 3px 0 rgba(92, 76, 76, 0.12);
  background-color: #f6f6f6;
  border-bottom-style: solid;
  border-bottom-width: 0.1rem;
  border-bottom-color: #a4a4a4;
  display: inline-block;
}

.tile.tile-active {
  background-color: #ededed;
  box-shadow: inset 0px 0px 6px #d3d3d3;
}

.tile:hover {
  background-color: #ededed;
}

.resources {
  display: inline-block;
  width: 38.332em;
  padding-left: 0.5rem;
}

.resources .title {
  border-bottom: solid 2px;
  border-bottom-color: #a4a4a4;
  padding-left: 0.5rem;
  padding-bottom: 0.5rem;
}
</style>