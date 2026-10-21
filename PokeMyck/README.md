# PokeMyck - A web application for view Pokemon information

In this app you can view information about different Pokemon, and you can also filter by type.
You can also view more details clicked on a Pokemon card.

Pagination
For this action, the app set per default 20 Pokemon per page.
The app apply the pagination based on the number of Pokemon available, and it will show the number of pages available, apply the next rule:
current page = total number of Pokemon / 20, rounded up to the nearest whole number.
The app receive the page number as a parameter in the URL, and it will show the corresponding page of Pokemon.
You can also navigate to the next and previous pages using the buttons provided.