import { Address } from "./address";

export class Exchange {
    exchangeCode: string = "";
    countryCode: string = "";

    name: string = "";
    description: string = "";
    address: Address = new Address();
}
