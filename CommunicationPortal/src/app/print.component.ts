
import { Component, ViewChild, OnInit, ComponentFactoryResolver, ApplicationRef, Injector, OnDestroy } from '@angular/core';
import { CdkPortal, DomPortalOutlet } from '@angular/cdk/portal';

/**
 * This component template wrap the projected content
 * with a 'cdkPortal'.
 */

@Component({
    selector: 'print',
    template: `
    <ng-container *cdkPortal>       
      <ng-content></ng-content>
    </ng-container>
  `
})
export class PrintComponent implements OnInit, OnDestroy {

    // STEP 1: get a reference to the portal
    @ViewChild(CdkPortal)
    portal!: CdkPortal;

    // STEP 2: save a reference to the window so we can close it
    private externalWindow: Window | null | undefined;

    // STEP 3: Inject all the required dependencies for a PortalHost
    constructor(
        private componentFactoryResolver: ComponentFactoryResolver,
        private applicationRef: ApplicationRef,
        private injector: Injector) { }

        ngAfterViewInit() {
             // STEP 4: create an external window
        this.externalWindow = window.open('', '', 'width=600,height=400,left=200,top=200');
        
                // STEP 5: create a PortalHost with the body of the new window document    
                if (this.externalWindow != null) {
                    const host = new DomPortalOutlet(
                        this.externalWindow.document.body,
                        this.componentFactoryResolver,
                        this.applicationRef,
                        this.injector
                    );
                    // STEP 6: Attach the portal
                    host.attach(this.portal);                   
                }
        
        
          }

    ngOnInit() {
       
    }

    ngOnDestroy() {
        // STEP 7: close the window when this component destroyed
        if (this.externalWindow != null)
            this.externalWindow.close()
    }
}