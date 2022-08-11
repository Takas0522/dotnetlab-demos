import { Auth } from "./auth";
import { Search } from "./search";

const auth = new Auth();
const searchClient = new Search(auth);

auth.loginRedirect();

const button = document.getElementById('eventsearch');
button?.addEventListener('click', async () => {
    await searchClient.eventSearchAsync();
});

const button2 = document.getElementById('extitemsearch');
button2?.addEventListener('click', async () => {
    await searchClient.externalItemSearchAsync();
});