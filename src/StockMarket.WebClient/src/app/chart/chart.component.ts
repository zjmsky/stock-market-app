import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";

import { Price } from "@root/_models/price";
import { PriceService } from "@root/_services/price.service";

@Component({
    templateUrl: "./chart.component.html",
    styleUrls: ["./chart.component.css"]
})
export class ChartComponent implements OnInit {
    private _priceService: PriceService;
    private _formBuilder: FormBuilder;

    public compareForm?: FormGroup;

    public barChartLabels = [""];
    public barChartData: Array<object> = [{ data: [], label: "" }];
    public barChartOptions = {
        scaleShowVerticalLines: false,
        responsive: true
    };
    public barChartColors = [
        { backgroundColor: "#ff7360" },
        { backgroundColor: "#6fc8ce" },
        { backgroundColor: "#fafff2" }
    ];

    constructor(priceService: PriceService, formBuilder: FormBuilder) {
        this._priceService = priceService;
        this._formBuilder = formBuilder;
    }

    ngOnInit() {
        this.compareForm = this._formBuilder.group({
            listing1: [""],
            listing2: [""],
            fromTime: ["", Validators.required],
            toTime: ["", Validators.required]
        });
    }

    onRequest() {
        if (this.compareForm?.invalid) return;

        this.barChartData = [];
        this.barChartLabels = [];

        const listing1 = this.compareForm!.get("listing1")!.value as string;
        const listing2 = this.compareForm!.get("listing2")!.value as string;
        const fromTime = new Date(this.compareForm!.get("fromTime")!.value);
        const toTime = new Date(this.compareForm!.get("toTime")!.value);

        if (listing1) {
            const [exch1, ticker1] = listing1.split(":");
            this._priceService
                .getList(exch1, ticker1, fromTime, toTime)
                .subscribe((data) => {
                    const dataArray = data.map((price) => price.currentPrice);
                    const timeArray = data.map((price) =>
                        new Date(price.time).toDateString()
                    );
                    this.barChartData.push({
                        data: dataArray,
                        label: listing1
                    });
                    this.barChartLabels = timeArray;
                });
        }

        if (listing2) {
            const [exch2, ticker2] = listing2.split(":");
            this._priceService
                .getList(exch2, ticker2, fromTime, toTime)
                .subscribe((data) => {
                    const dataArray = data.map((price) => price.currentPrice);
                    const timeArray = data.map((price) =>
                        new Date(price.time).toDateString()
                    );
                    this.barChartData.push({
                        data: dataArray,
                        label: listing2
                    });
                    this.barChartLabels = timeArray;
                });
        }
    }
}
