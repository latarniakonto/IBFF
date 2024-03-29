<template>
  <div class="resources-tab mb-2">
    <div class="card">
      <div class="table-toolbar">
        <Toolbar class="mb-4">
          <template #start>
            <Button
              label="Transaction"
              icon="pi pi-plus"
              class="p-button-success mr-2"
              @click="addTransaction()"
            />
          </template>
        </Toolbar>
      </div>
      <TransactionsTable 
        :transactions="transactions"
        @transactionDeleted="handleTransactionDeleted($event)"
        @transactionEdited="handleTransactionEdited($event)"
      />
    </div>
    <AddTransactionDialog
      :addTransactionDialog="addTransactionDialog"
      @transactionAdded="handleTransactionAdded($event)"
      @transactionAddingCanceled="handleTransactionAddingCanceled()"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Button from "primevue/button";
import Toolbar from "primevue/toolbar";
import TransactionsTable from "./TransactionsTable.vue";
import AddTransactionDialog from "./AddTransactionDialog.vue";
import { Transaction, TransactionJSON } from "../../common/models";
import { axios } from "../../common/api.service";
import { toastSuccess, toastError } from "../../common/api.toast";

export default defineComponent({
  name: "TransactionsTab",

  components: {
    Button,
    Toolbar,
    TransactionsTable,
    AddTransactionDialog,
  },

  data() {
    return {
      addTransactionDialog: false as Boolean,      
      transactions: [] as Array<Transaction>,
    };
  },

  methods: {
    addTransaction() {
      if (this.$store.state.portfolio.slug === undefined) {
        return;
      }

      this.addTransactionDialog = true;
    },

    async handleTransactionAdded(transaction: Transaction) {
      transaction.portfolioSlug = this.$store.state.portfolio.slug;
      if (await this.performTransactionCreateRequest(transaction)) {
        this.transactions.push(transaction);
      }

      this.addTransactionDialog = false;
    },

    handleTransactionAddingCanceled() {
      this.addTransactionDialog = false;
    },

    async handleTransactionDeleted(transaction: Transaction) {
      if (await this.performTransactionDeleteRequest(transaction)) {
        this.transactions = this.transactions.filter((val) => val.id !== transaction.id);
      }
    },

    async handleTransactionEdited(transaction: Transaction, index: number) {      
      if (await this.performTransactionPutRequest(transaction)) {
        this.transactions.splice(index, 1, transaction);
      }
    },

    async getTransactions() {
      if (this.$store.state.portfolio.slug === undefined) {
        return;
      }

      let endpoint = `api/v1/portfolios/${this.$store.state.portfolio.slug}/transactions/`;

      try {
        let response = await axios.get(endpoint);
        let jsons = response.data;
        jsons.forEach((json: any) => {
          this.transactions.push(new Transaction(json as TransactionJSON));
        });
      } catch (e: any) {
        console.error(e.response);
      }
    },

    async performTransactionCreateRequest(transaction: Transaction): Promise<Boolean> {
      let endpoint = "/api/v1/transactions/";
      let method = "POST";
      try {
        const response = await axios({
          method: method,
          url: endpoint,
          data: transaction,
        });
        transaction.id = response.data.uutid;
        transaction.slug = response.data.slug;
        toastSuccess("Transaction created.", 3000);

        return true;
      } catch (e: any) {
        console.error(e.response.statusText);
        toastError("Transaction was not created.", 3000);
        
        return false;
      }
    },

    async performTransactionDeleteRequest(transaction: Transaction): Promise<Boolean> {
      let endpoint = `/api/v1/transactions/${transaction.slug}/`;

      try {
        const response = await axios.delete(endpoint);
        toastSuccess("Transaction deleted.", 3000);

        return true;
      } catch (e: any) {
        console.error(e.response.statusText);
        toastError("Transaction was not deleted.", 3000)

        return false;
      }
    },

    async performTransactionPutRequest(transaction: Transaction): Promise<Boolean> {
      let endpoint = `/api/v1/transactions/${transaction.slug}/`;
      let method = "PUT";
      try {
        const response = await axios({
          method: method,
          url: endpoint,
          data: transaction,
        });
        toastSuccess("Transaction modified.", 3000);

        return true;
      } catch (e: any) {
        console.error(e.response.statusText);
        toastError("Transaction was not modified.", 3000);

        return false;
      }
    },
  },

  created() {
    this.getTransactions();
  },
});
</script>

<style>
.resources-tab .table-toolbar {
  background-color: #f6f6f6;
  height: 60px;
}

.resources-tab .table-toolbar .p-toolbar {
  background-color: #f6f6f6;
  border-color: #a4a4a4;
  padding: 0.3rem 0 0 0.3rem;
}
</style>
