﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public OrderStatus Status { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();

        public ICollection<FoodOrder> Food { get; set; } = new HashSet<FoodOrder>();

        public ICollection<ToyOrder> Toys { get; set; } = new HashSet<ToyOrder>();
    }
}
