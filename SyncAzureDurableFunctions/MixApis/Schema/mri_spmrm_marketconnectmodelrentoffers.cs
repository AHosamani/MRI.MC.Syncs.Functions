﻿namespace MRI.PandA.Syncs.Functions.MixApis.Schema;
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.18020.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute("mri_s-pmrm_marketconnectmodelrentoffers", Namespace = "", IsNullable = false)]
public partial class mri_spmrm_marketconnectmodelrentoffers {

  private mri_spmrm_marketconnectmodelrentoffersEntry[] entryField;

  private string nextPageLinkField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("entry")]
  public mri_spmrm_marketconnectmodelrentoffersEntry[] entry {
    get {
      return this.entryField;
    }
    set {
      this.entryField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
  public string NextPageLink {
    get {
      return this.nextPageLinkField;
    }
    set {
      this.nextPageLinkField = value;
    }
  }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class mri_spmrm_marketconnectmodelrentoffersEntry {

  private string rentOfferIDField;

  private string floorplanIDField;

  private string rentTermField;

  private string startDateField;

  private string endDateField;

  private string offerBaseRentField;

  private string concessionField;

  /// <remarks/>
  public string RentOfferID {
    get {
      return this.rentOfferIDField;
    }
    set {
      this.rentOfferIDField = value;
    }
  }

  /// <remarks/>
  public string FloorplanID {
    get {
      return this.floorplanIDField;
    }
    set {
      this.floorplanIDField = value;
    }
  }

  /// <remarks/>
  public string RentTerm {
    get {
      return this.rentTermField;
    }
    set {
      this.rentTermField = value;
    }
  }

  /// <remarks/>
  public string StartDate {
    get {
      return this.startDateField;
    }
    set {
      this.startDateField = value;
    }
  }

  /// <remarks/>
  public string EndDate {
    get {
      return this.endDateField;
    }
    set {
      this.endDateField = value;
    }
  }

  /// <remarks/>
  public string OfferBaseRent {
    get {
      return this.offerBaseRentField;
    }
    set {
      this.offerBaseRentField = value;
    }
  }

  /// <remarks/>
  public string Concession {
    get {
      return this.concessionField;
    }
    set {
      this.concessionField = value;
    }
  }
}
