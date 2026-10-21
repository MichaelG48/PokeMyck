# PokeMyck - A web application for view Pokemon information

In this app you can view information about different Pokemon, and you can also filter by type.
You can also view more details clicked on a Pokemon card.

Get data
This app get the information about Pokemon from the [PokeAPI](https://pokeapi.co/).


Pagination
For this action, the app set per default 20 Pokemon per page.
The app apply the pagination based on the number of Pokemon available, and it will show the number of pages available, apply the next rule:
current page = total number of Pokemon / 20, rounded up to the nearest whole number.
The app receive the page number as a parameter in the URL, and it will show the corresponding page of Pokemon.
You can also navigate to the next and previous pages using the buttons provided.
This function was implemented by this way because the PokeAPI does not provide a way to get a total number of page, and it is not efficient to get all Pokemon 
at once because it would take a long time to load the page. So for this reason, the app get the total number of Pokemon, calculated the number of page and 
apply the pagination based on the limit of pokemon per page.


Download pokemon list
You can download the list of Pokemon in XLS format by clicking on the "Download" button. The CSV file will contain the following information for each Pokemon:
Name, ImageUrl


Send by Email
If you want to send the list of Pokemon by email, you can need configure the email settings in the apo in the SendByEmail method into HomeController.
You need to set the following parameters:
EmailFrom: The email address from which the email will be sent.
AppPassword: The password for the email account.


