<p align="center">
  <img src="assets/banner.png" alt="Jellygram" width="800">
</p>

<h1 align="center">Jellygram Italiano</h1>

<p align="center">
  Notifiche Telegram ricche e raggruppate per i nuovi film e le nuove serie TV aggiunti su Jellyfin.
</p>

<p align="center">
  <a href="https://github.com/fixx1983/jellyfin-plugin-jellygram/actions/workflows/ci.yml"><img src="https://github.com/fixx1983/jellyfin-plugin-jellygram/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="Licenza MIT"></a>
  <img src="https://img.shields.io/badge/Jellyfin-12.0.0--rc2-00A4DC" alt="Jellyfin 12.0.0-rc2">
</p>

> **Nota:** Questo progetto è un fork in italiano del plugin originale [jellyfin-plugin-jellygram](https://github.com/TheCommishDeuce/jellyfin-plugin-jellygram) realizzato da **TheCommishDeuce**.

Jellygram gira direttamente all'interno di Jellyfin e ascolta in tempo reale le aggiunte alla libreria. Attende che le importazioni multiple si stabilizzino, ne risolve la gerarchia e invia aggiornamenti Telegram concisi senza richiedere webhook, servizi secondari o endpoint pubblici.

## Funzionalità

- Notifiche per film, serie TV, stagioni ed episodi
- Locandine e trame recuperate dai metadati di Jellyfin
- Raggruppamento gerarchico che evita l'invio di un messaggio per ogni singolo episodio
- Finestra di raggruppamento dinamica per importazioni lente o scaglionate
- Tentativi automatici di attesa per gli episodi i cui metadati sono ancora in fase di identificazione
- Supporto a gruppi, canali e argomenti (topic) dei forum di Telegram
- Invio silenzioso opzionale
- Configurazione persistente direttamente dalla dashboard di Jellyfin
- Escape HTML e nuovi tentativi automatici in caso di errore di invio

## Gerarchia delle notifiche

| Priorità | Contenuto aggiunto su Jellyfin | Risultato su Telegram |
|---:|---|---|
| 1 | Intera nuova serie | Un unico riepilogo della serie che ingloba stagioni ed episodi |
| 2 | Nuova stagione di una serie esistente | Un unico riepilogo della stagione che ingloba gli episodi |
| 3 | Più episodi di una serie esistente | Un unico riepilogo raggruppato con gli intervalli degli episodi |
| 4 | Un singolo episodio di una serie esistente | Una notifica dettagliata per il singolo episodio |
| — | Film | Una notifica dettagliata per il film |

Le aggiunte di livello superiore hanno la precedenza: importare una nuova serie non genera anche notifiche separate per stagioni ed episodi. Le locandine vengono allegate quando disponibili; gli esempi testuali qui sotto le omettono.

### Film

~~~text
🎬 Nuovo film
Cars (2006)

Trama
Un'auto da corsa esordiente e ambiziosa rimane bloccata in una cittadina dimenticata e scopre che vincere non è tutto.
~~~

### Nuova serie

Se il primo contenuto rilevato da Jellyfin appartiene a una stagione successiva alla prima, Jellygram la identifica correttamente invece di indicare genericamente "1 stagione":

~~~text
📺 Nuova serie
The Bear (2022)
Stagione 05 · 8 episodi

Trama
Un giovane chef dell'alta ristorazione torna a Chicago per gestire la paninoteca di famiglia.
~~~

Un'importazione di più stagioni utilizza un intervallo compatto come `Stagioni 01–03 · 28 episodi`.

### Nuova stagione di una serie esistente

~~~text
📚 Nuova stagione
For All Mankind — Stagione 03
10 episodi inclusi

Trama
Il Pianeta Rosso diventa la nuova frontiera nella corsa allo spazio.
~~~

La notifica di nuova stagione ingloba tutti gli episodi di quella stagione importati nello stesso blocco.

### Più episodi di una serie esistente

~~~text
🍿 Nuovi episodi
For All Mankind · 3 episodi
- S03: E08–E10 — The Sands of Ares, Coming Home, Stranger in a Strange Land
~~~

Gli episodi non consecutivi vengono elencati in modo esplicito, ad esempio `S04: E01–E03, E05`.

### Singolo episodio di una serie esistente

~~~text
🍿 Nuovo episodio
For All Mankind
S03E08 — The Sands of Ares

Trama
La Helios e la missione sovietica fanno una scoperta inaspettata.
~~~

## Compatibilità

La versione `1.1.0.0` è compilata per:

- Jellyfin `12.0.0-rc2` (e versioni `12.x`)
- .NET `10.0`

I plugin di Jellyfin dipendono dalla versione specifica dell'ABI. Non installare questa build su Jellyfin 10.10 o 10.11.

## Installazione

### Repository dei plugin (consigliato)

Aggiungi questo URL in **Dashboard → Plugin → Repository**:

~~~text
https://raw.githubusercontent.com/fixx1983/jellyfin-plugin-jellygram/main/manifest.json
~~~

Successivamente installa **Jellygram Italiano** dal catalogo dei plugin e riavvia Jellyfin.

### Installazione manuale su Docker

1. Scarica `jellygram-italiano_1.1.0.0.zip` dalla sezione Releases su GitHub.
2. Estrai `Jellyfin.Plugin.Jellygram.dll` nel volume di configurazione di Jellyfin:

   ~~~text
   /config/plugins/Jellygram/Jellyfin.Plugin.Jellygram.dll
   ~~~

3. Riavvia il container di Jellyfin.

Per il deploy locale in fase di sviluppo da questo repository:

~~~bash
./scripts/deploy-docker.sh root@tuo-host-jellyfin
~~~

Lo script compila localmente, individua il container Jellyfin attivo, copia la DLL e riavvia il container.

## Configurazione di Telegram

1. Scrivi a [@BotFather](https://t.me/BotFather), invia `/newbot` e copia il token del bot.
2. Aggiungi il bot al gruppo o al canale di destinazione. Nel caso di un canale, concedigli i permessi per pubblicare messaggi.
3. Invia un messaggio nella chat di destinazione.
4. Apri `https://api.telegram.org/bot<TOKEN>/getUpdates` e individua il valore di `message.chat.id` oppure `channel_post.chat.id`.
5. Per gli argomenti (topic) dei forum, utilizza il valore `message_thread_id` di un aggiornamento inviato all'interno del topic.

Tratta il token del bot come una password. Se viene esposto pubblicamente, rigeneralo tramite BotFather.

## Configurazione del plugin

Apri **Dashboard → Plugin → I miei plugin → Jellygram Italiano**.

| Impostazione | Descrizione |
|---|---|
| Telegram bot token | Token rilasciato da BotFather |
| Chat ID | ID dell'utente, gruppo, supergruppo o canale; gli ID dei gruppi sono solitamente negativi |
| Topic/thread ID | Argomento (topic) opzionale di un forum Telegram |
| Batch window | Secondi di inattività prima dell'invio; ogni nuovo elemento azzera il timer |
| Include descriptions | Include la trama utilizzando il metadato `Overview` di Jellyfin |
| Include posters | Carica l'immagine principale di Jellyfin quando il limite di caratteri della didascalia Telegram lo consente |
| Send silently | Invia la notifica senza suono |

Una finestra di `90` secondi è adatta alle importazioni normali. Per intere stagioni in 4K di grandi dimensioni copiate in sequenza, è preferibile impostare `300`–`600` secondi.

## Come funziona il raggruppamento

Jellygram avvia un timer di inattività quando Jellyfin genera l'evento `ItemAdded`. Ulteriori aggiunte azzerano il timer. Allo scadere della finestra, Jellygram ricarica gli elementi da Jellyfin e costruisce il set minimo e più utile di notifiche.

Se a un episodio mancano ancora l'ID della serie, l'ID della stagione, il numero di stagione o il numero di episodio, l'intero blocco attende e riprova a risolvere i metadati ogni minuto. Dopo dieci tentativi invia ciò che è disponibile e registra un avviso nei log, evitando che un singolo file malformato blocchi le notifiche all'infinito.

Attualmente Jellygram monitora solo i nuovi elementi creati su Jellyfin. L'aggiunta di una versione alternativa (es. 4K) a un episodio già esistente genera un evento `ItemUpdated` anziché `ItemAdded` e intenzionalmente non produce notifiche.

## Log

Jellygram utilizza il normale sistema di logging di Jellyfin. Su Docker:

~~~bash
docker logs --follow jellyfin 2>&1 | grep --line-buffered -iE 'jellygram|telegram'
~~~

Un invio riuscito appare così nei log:

~~~text
Jellyfin.Plugin.Jellygram.Notifications.LibraryNotifier: Sent 1 Telegram notifications for 14 library items
~~~

## Compilazione e test

Requisiti: .NET 10 SDK.

~~~bash
dotnet restore Jellyfin.Plugin.Jellygram.sln
dotnet test Jellyfin.Plugin.Jellygram.sln -c Release
dotnet build Jellyfin.Plugin.Jellygram.sln -c Release --no-restore
~~~

Creazione dell'archivio di rilascio e del checksum MD5:

~~~bash
./scripts/package.sh
~~~

## Privacy e sicurezza

Jellygram invia esclusivamente il testo configurato della notifica e la locandina selezionata alle API ufficiali di Telegram Bot. Non espone alcun endpoint HTTP in ingresso né contatta direttamente provider esterni di metadati. Il token del bot è salvato nella configurazione dei plugin di Jellyfin e non viene mai registrato intenzionalmente nei log.

Consulta [SECURITY.md](SECURITY.md) per segnalare eventuali vulnerabilità.

## Licenza

[MIT](LICENSE)
