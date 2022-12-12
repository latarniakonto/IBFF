<template>
    <div class="transactions-table">
      <Panel :collapsed="transactionsTableCollapsed">
        <template #header>
          <div
            class="transactions-table-panel-header"
            @click="toggleTransactionsTable()"
          >
            <span class="p-panel-title">Transactions</span>
          </div>
        </template>
        <template #icons>
          <div
            class="transactions-table-panel-header"
            @click="toggleTransactionsTable()"
          >
            <div class="p-panel-header-icon p-link mr-2">
              <span v-if="transactionsTableCollapsed" class="pi pi-plus"></span>
              <span v-else class="pi pi-minus"></span>
            </div>
          </div>
        </template>
        <div class="card">
          <DataTable
            :value="transactions"
            editMode="row"
            dataKey="id"
            v-model:editingRows="editingTransactions"
            :scrollable="true"
          >
            <Column field="date" header="Date">
              <template #body="slotProps">
                {{ getPrintableDate(slotProps.data.date) }}
              </template>
              <template #editor="{ data, field }">
                <Calendar
                  v-model="data[field]"
                  dateFormat="dd.mm.yy"
                  autofocus
                />
              </template>
            </Column>
            <Column field="type" header="Type">
              <template #editor="{ data, field }">
                <Dropdown
                  v-model="data[field]"
                  :options="transactionTypes"
                  optionLabel="label"
                  optionValue="value"
                  :placeholder="data[field]"
                >
                  <template #option="slotProps">
                    <span
                      :class="
                        'product-badge status-' +
                        slotProps.option.value.toLowerCase()
                      "
                      >{{ slotProps.option.label }}</span
                    >
                  </template>
                </Dropdown>
              </template>
            </Column>
            <Column field="price" header="Price">
              <template #editor="{ data, field }">
                <InputNumber
                  id="price"
                  v-model="data[field]"
                  mode="decimal"
                  :min="0.001"
                  :maxFractionDigits="3"
                  autofocus
                  required="true"
                />
              </template>
            </Column>
            <Column field="amount" header="Amount">
              <template #editor="{ data, field }">
                <InputNumber
                  id="amount"
                  v-model="data[field]"
                  integeronly
                  :min="1"
                  autofocus
                  required="true"
                />
              </template>
            </Column>
            <Column field="provision" header="Provision">
              <template #editor="{ data, field }">
                <InputNumber
                  id="provision"
                  v-model="data[field]"
                  mode="decimal"
                  :min="0"
                  :maxFractionDigits="2"
                  required="true"
                />
              </template>
            </Column>
            <Column bodyStyle="text-align:center">
              <template #body="slotProps">
                <button
                  type="button"
                  class="btn btn-warning btn-sm mr-1"
                  @click="editTransaction(slotProps.data)"
                >
                  <i class="bi bi-pencil-fill"></i>
                </button>
                <button
                  type="button"
                  class="btn btn-danger btn-sm"
                  @click="deleteTransaction(slotProps.data)"
                >
                  <i class="bi bi-trash3-fill"></i>
                </button>
              </template>
              <template #editor="slotProps">
                <button
                  type="button"
                  class="btn btn-warning btn-sm mr-1"
                  @click="submitTransaction(slotProps.data, slotProps.index)"
                >
                  <i class="bi bi-check"></i>
                </button>
                <button
                  type="button"
                  class="btn btn-warning btn-sm"
                  @click="revertTransaction(slotProps.data)"
                >
                  <i class="bi bi-x"></i>
                </button>
              </template>
            </Column>
          </DataTable>
        </div>
      </Panel>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Button from "primevue/button";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Row from "primevue/row";
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import Dropdown from "primevue/dropdown";
import Calendar from "primevue/calendar";
import Panel from "primevue/panel";

export default defineComponent({
  name: "TransactionsPanel",

  components: {
    Button,
    DataTable,
    Column,
    Row,
    InputText,
    Dropdown,
    InputNumber,
    Calendar,
    Panel,
  },

  data() {
    return {
      transactionTypes: [
        { label: "Buy", value: "Buy" },
        { label: "Sell", value: "Sell" },
      ] as Array<any>,
      transactions: [
        {
          id: "1000",
          date: new Date("2020-01-30") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 20 as Number,
          provision: 1.9 as Number,
        },
        {
          id: "1001",
          date: new Date("2020-08-12") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 22 as Number,
          provision: 1.9 as Number,
        },
        {
          id: "1002",
          date: new Date("2020-08-12") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 13 as Number,
          provision: 0 as Number,
        },
        {
          id: "1003",
          date: new Date("2021-04-07") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 70 as Number,
          provision: 5 as Number,
        },
        {
          id: "1004",
          date: new Date("2021-10-26") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 75 as Number,
          provision: 5.06 as Number,
        },
        {
          id: "1005",
          date: new Date("2022-02-24") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 100 as Number,
          provision: 5.42 as Number,
        },
        {
          id: "1006",
          date: new Date("2022-02-24") as Date,
          type: "Buy" as String,
          price: 0 as Number,
          amount: 50 as Number,
          provision: 5 as Number,
        },
      ] as Array<any>,
      editingTransactions: [] as Array<any>,
      transactionsTableCollapsed: true as Boolean,
    };
  },

  methods: {
    getPrintableDate(date: Date) {
      return (
        date.getDate().toString() +
        "." +
        (date.getMonth() + 1).toString() +
        "." +
        date.getFullYear().toString()
      );
    },

    editTransaction(transaction: any) {
      this.editingTransactions = [transaction];
    },

    deleteTransaction(transaction: any) {
      this.transactions = this.transactions.filter(
        (val) => val.id !== transaction.id
      );
      this.editingTransactions = [];

      this.$toast.add({
        severity: "success",
        summary: "Successful",
        detail: "Transaction Deleted",
        life: 3000,
      });
    },

    submitTransaction(transaction: any, index: number) {
      this.transactions[index] = transaction;
      this.editingTransactions = [];

      this.$toast.add({
        severity: "success",
        summary: "Successful",
        detail: "Transaction Modified",
        life: 3000,
      });
    },

    revertTransaction(transaction: any) {
      this.editingTransactions = [];
    },

    toggleTransactionsTable() {
      this.transactionsTableCollapsed = !this.transactionsTableCollapsed;
    },
  },
});
</script>

<style>
.transactions-table .p-datatable-tbody {
  min-height: 27.2rem;
  max-height: 27.2rem;
}

.transactions-table .p-datatable .p-datatable-thead tr th {
  justify-content: end;
  background-color: #f6f6f6;
}

.transactions-table .p-datatable .p-datatable-tbody tr td {
  justify-content: end;
  padding-top: 0.7rem;
  padding-bottom: 0.7rem;
}

.transactions-table .card {
  border-color: #a4a4a4;
}

.transactions-table .p-inputnumber .p-inputnumber-input {
  width: 7rem;
  height: 3rem;
  padding: 0.5rem;
}

.transactions-table .p-calendar .p-inputtext {
  width: 6.5rem;
  height: 3rem;
  padding: 0.5rem;
}

.transactions-table .p-dropdown .p-dropdown-label {
  width: 3rem;
  height: 3rem;
  padding: 0.5rem;
}

.transactions-table .p-panel .p-panel-header {
  padding: 0;
  background-color: #f6f6f6;
  cursor: pointer;
  border-bottom-style: solid;
  border-bottom-width: 0.1rem;
  border-bottom-color: #a4a4a4;
  align-items: stretch;
}

.transactions-table .p-panel .p-panel-header:hover {
  background-color: #ededed;
}

.transactions-table .p-panel .p-panel-header .p-panel-header-icon {
  pointer-events: none;
}

.transactions-table .p-panel .p-panel-content {
  padding: 0;
}

.transactions-table .transactions-table-panel-header {
  padding: 1rem;
  flex: 1;
}

.transactions-table .transactions-table-panel-header .p-panel-title {
  line-height: 2em;
}
</style>